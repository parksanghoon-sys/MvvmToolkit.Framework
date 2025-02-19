using Microsoft.Extensions.Logging;
using Moq;
using MvvmToolkit.Core.Tasks.Worker;
using PeriodicTimer = MvvmToolkit.Core.Threadings.PeriodicTimer;

namespace Toolkit.Test
{
    public class PeriodicTimerWorkerTests
    {
        private readonly Mock<ILogger<PeriodicTimerWorker>> _loggerMock;
        private readonly PeriodicTimerWorker _worker;
        public PeriodicTimerWorkerTests()
        {
            _loggerMock = new Mock<ILogger<PeriodicTimerWorker>>();
            _worker = new PeriodicTimerWorker("TestTimer", _loggerMock.Object);
        }
        [Fact]
        public async Task Worker_Should_Execute_Task_Per_Cycle()
        {
            // Arrange
            var executedCount = 0;
            var cycleTime = TimeSpan.FromMilliseconds(100);
            Func<Task> work = async () =>
            {
                executedCount++;
                await Task.Delay(10);
            };

            // Act
            _worker.StartWorker(cycleTime, work);
            await Task.Delay(350);
            _worker.StopWorker();

            // Assert
            Assert.InRange(executedCount, 2, 4); // Ensure it executed multiple times
        }

        [Fact]
        public async Task Worker_Should_Stop_Correctly()
        {
            // Arrange
            var executed = false;
            Func<Task> work = async () =>
            {
                executed = true;
                await Task.Delay(10);
            };

            _worker.StartWorker(TimeSpan.FromMilliseconds(100), work);
            await Task.Delay(150);
            _worker.StopWorker();

            // Act
            executed = false;
            await Task.Delay(200);

            // Assert
            Assert.False(executed, "Task should not run after stopping");
        }
    }
    public class PeriodicTimerTests
    {
        [Fact]
        public async Task PeriodicTimer_Should_Tick_At_Interval()
        {
            // Arrange
            var cycleTime = TimeSpan.FromMilliseconds(100);
            using var timer = new PeriodicTimer(cycleTime);
            var cts = new CancellationTokenSource();

            // Act
            var firstTick = await timer.WaitForNextTickAsync(cts.Token);
            var secondTick = await timer.WaitForNextTickAsync(cts.Token);

            // Assert
            Assert.True(firstTick);
            Assert.True(secondTick);
        }

        [Fact]
        public async Task PeriodicTimer_Should_Stop_On_Dispose()
        {
            // Arrange
            var cycleTime = TimeSpan.FromMilliseconds(100);
            var timer = new PeriodicTimer(cycleTime);
            var cts = new CancellationTokenSource();

            // Act
            var firstTick = await timer.WaitForNextTickAsync(cts.Token);
            timer.Dispose();
            await Task.Delay(150);
            var secondTick = timer.WaitForNextTickAsync(cts.Token);

            // Assert
            Assert.True(firstTick);
            Assert.True(secondTick.IsCompletedSuccessfully);
            //await Assert.ThrowsAsync<OperationCanceledException>(async () => await secondTick);
        }
        [Fact]
        public async Task PeriodicTimer_Should_Cancel_On_Token()
        {
            // Arrange
            var cycleTime = TimeSpan.FromMilliseconds(100);
            using var timer = new PeriodicTimer(cycleTime);
            var cts = new CancellationTokenSource();

            // Act
            var firstTick = await timer.WaitForNextTickAsync(cts.Token);
            cts.Cancel();
            await Task.Delay(150);
            var secondTick = timer.WaitForNextTickAsync(cts.Token);

            // Assert
            Assert.True(firstTick);
            Assert.False(secondTick.IsCompletedSuccessfully);            
        }
    }

}
