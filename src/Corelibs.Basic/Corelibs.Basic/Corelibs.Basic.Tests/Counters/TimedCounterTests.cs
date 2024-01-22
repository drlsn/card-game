using Corelibs.Basic.Counters;
using NUnit.Framework;

namespace Common.Basic.Tests
{
    internal class TimedCounterTests
    {
        [Test]
        public void GivenTimedCounter_WhenReachCountIsOneIntervalResetTimeNotPassed_ThenShouldReachCount()
        {
            // Arrange
            var timer = new TimerFunctions();
            timer.ElapsedMilliseconds = () => 699;
            timer.IsRunning = () => true;

            bool countReached = false;
            var counter = new TimedCounter(1, 700, () => countReached = true, timer);

            // Act
            counter.Increase();

            // Assert
            Assert.IsTrue(countReached);
        }

        [Test]
        public void GivenTimedCounter_WhenReachCountIsThreeIntervalResetTimeNotPassed_ThenShouldReachCount()
        {
            // Arrange
            var timer = new TimerFunctions();
            timer.ElapsedMilliseconds = () => 699;
            timer.IsRunning = () => true;

            bool countReached = false;
            var counter = new TimedCounter(3, 700, () => countReached = true, timer);

            // Act
            counter.Increase();
            counter.Increase();
            counter.Increase();

            // Assert
            Assert.IsTrue(countReached);
        }

        [Test]
        public void GivenTimedCounter_WhenReachCountMoreThanOne_ThenShouldNotReachCount()
        {
            // Arrange
            var timer = new TimerFunctions();
            timer.IsRunning = () => true;

            bool countReached = false;
            var counter = new TimedCounter(3, 700, () => countReached = true, timer);

            // Act
            counter.Increase();

            // Assert
            Assert.IsFalse(countReached);
        }

        [Test]
        public void GivenTimedCounter_WhenReachCountIsOneIntervalResetTimePassed_ThenShouldNotReachCount()
        {
            // Arrange
            var timer = new TimerFunctions();
            timer.ElapsedMilliseconds = () => 700;
            timer.IsRunning = () => true;

            bool countReached = false;
            var counter = new TimedCounter(1, 700, () => countReached = true, timer);

            // Act
            counter.Increase();

            // Assert
            Assert.IsFalse(countReached);
        }
    }
}