namespace MvvmToolkit.Core.Logs
{
    public enum LogOutputType
    {
        Console,
        File,
        Tcp
    }
    public class LoggerOptions
    {
        public LogOutputType OutputType { get; set; } = LogOutputType.Console;
        public string? FilePath { get; set; } = "log.txt";
        public string? TcpHost { get; set; }
        public int TcpPort { get; set; } = 5000;
    }
}
