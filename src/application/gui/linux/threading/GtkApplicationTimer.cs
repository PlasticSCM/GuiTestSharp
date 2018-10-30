using Codice.Examples.GuiTesting.Lib.Threading;

namespace Codice.Examples.GuiTesting.Linux.Threading
{
    internal class GtkApplicationTimerBuilder : IApplicationTimerBuilder
    {
        public IApplicationTimer Get(
            bool bModalMode,
            ThreadWaiter.TimerTick timerTickDelegate)
        {
            return new GtkApplicationTimer(DEFAULT_TIMER_INTERVAL, timerTickDelegate);
        }

        public IApplicationTimer Get(
            bool bModalMode,
            int timerInterval,
            ThreadWaiter.TimerTick timerTickDelegate)
        {
            return new GtkApplicationTimer((uint)timerInterval, timerTickDelegate);
        }

        const int DEFAULT_TIMER_INTERVAL = 500;
    }

    internal class GtkApplicationTimer : IApplicationTimer
    {
        internal GtkApplicationTimer(
            uint timerInterval, ThreadWaiter.TimerTick timerTickDelegate)
        {
            mTimerInterval = timerInterval;
            mTimerTickDelegate = timerTickDelegate;
        }

        public void Start()
        {
            mTimeoutId = GLib.Timeout.Add(
                mTimerInterval, new GLib.TimeoutHandler(OnTimerTick));
        }

        public void Stop()
        {
            if (mTimeoutId == 0)
                return;

            GLib.Source.Remove(mTimeoutId);
            mTimeoutId = 0;
        }

        bool OnTimerTick()
        {
            mTimerTickDelegate();

            return true;
        }

        uint mTimeoutId;

        readonly uint mTimerInterval;
        ThreadWaiter.TimerTick mTimerTickDelegate;
    }
}
