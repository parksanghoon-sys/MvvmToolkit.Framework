using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmToolkit.Core.Logs;
using System.Net.Sockets;
using System.Text;

namespace Toolkit.LoggerTest
{
    class A
    {
        public int a;
    }

    public class CustomLoggerTests : IDisposable
    {
        public CustomLoggerTests()
        {
            A classA = new A();
            classA.a = 10;

            A classB = classA;
            classB.a = 20;

            Console.WriteLine(classA.a);
        }
        private readonly string _testFilePath = "test_log.txt";

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
        private LoggerOptions CreateLoggerOptions(LogOutputType outputType, string? filePath = null)
        {
            return new LoggerOptions
            {
                OutputType = outputType,
                FilePath = filePath ?? _testFilePath
            };
        }

        private CustomLogger CreateLogger(LogOutputType outputType)
        {
            var options = Options.Create(CreateLoggerOptions(outputType));
            return new CustomLogger("TestLogger", options);
        }

        [Fact]
        public void ConsoleLogging_WritesToConsole()
        {
            string a = "ABC";
            string  b =  a;
            b = "B";
            Console.WriteLine(a);
            // Arrange
            var logger = CreateLogger(LogOutputType.Console);
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            logger.Log(LogLevel.Information, new EventId(1), "Test console log", null, (state, ex) => state.ToString());

            // Assert
            var logOutput = output.ToString();
            Assert.Contains("[Information] TestLogger - Test console log", logOutput);
        }

        [Fact]
        public void FileLogging_WritesToFile()
        {
            // Arrange
            var logger = CreateLogger(LogOutputType.File);
            if (File.Exists(_testFilePath)) File.Delete(_testFilePath);

            // Act
            logger.Log(LogLevel.Warning, new EventId(2), "Test file log", null, (state, ex) => state.ToString());

            // Assert
            Assert.True(File.Exists(_testFilePath));
            var fileContents = File.ReadAllText(_testFilePath);
            Assert.Contains("[Warning] TestLogger - Test file log", fileContents);
        }

        [Fact]
        public async Task TcpLogging_SendsDataToTcpServer()
        {
            // Arrange
            var tcpHost = "localhost";
            var tcpPort = 5000;

            var options = Options.Create(new LoggerOptions
            {
                OutputType = LogOutputType.Tcp,
                TcpHost = tcpHost,
                TcpPort = tcpPort
            });

            var logger = new CustomLogger("TestLogger", options);

            using var mockTcpListener = new TcpListener(System.Net.IPAddress.Loopback, tcpPort);
            mockTcpListener.Start();

            // Act
            logger.Log(LogLevel.Error, new EventId(3), "Test TCP log", null, (state, ex) => state.ToString());

            using var client = await mockTcpListener.AcceptTcpClientAsync();
            using var stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Assert
            Assert.Contains("[Error] TestLogger - Test TCP log", receivedMessage);
        }
    }
}