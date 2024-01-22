using System;
using System.Diagnostics;

namespace Corelibs.Basic.Counters
{
    public class TimedCounter
    {
        private readonly int _reachCount;
        private readonly float _increaseIntervalResetMS;
        private readonly Action _onCountReached;

        private readonly TimerFunctions _timer;
        private int _count;

        public TimedCounter(
            int reachCount, 
            float increaseIntervalResetMS, 
            Action onCountReached,
            TimerFunctions timer)
        {
            _reachCount = reachCount;
            _increaseIntervalResetMS = increaseIntervalResetMS;
            _onCountReached = onCountReached;

            _timer = timer;
        }

        public void Increase()
        {
            if (!_timer.IsRunning())
                _timer.Restart();

            _count++;

            if (_timer.ElapsedMilliseconds() >= _increaseIntervalResetMS)
            {
                Reset();
                return;
            }

            if (_count == _reachCount)
            {
                _onCountReached();
                Reset();
            }
        }

        private void Reset()
        {
            _timer.Stop();
            _count = 0;
        }
    }

    public class TimerFunctions
    {
        public Action Restart = () => { };
        public Action Stop = () => { };
        public Func<bool> IsRunning = () => false;
        public Func<float> ElapsedMilliseconds = () => 0.0f;

        public static TimerFunctions FromStopwatch()
        {
            var stopwatch = new Stopwatch();

            var tf = new TimerFunctions();
            tf.Restart = stopwatch.Restart;
            tf.Stop = stopwatch.Stop;
            tf.IsRunning = () => stopwatch.IsRunning;
            tf.ElapsedMilliseconds = () => stopwatch.ElapsedMilliseconds;

            return tf;
        }
    }
}
