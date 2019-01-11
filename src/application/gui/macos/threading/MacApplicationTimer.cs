using Foundation;

using Codice.Examples.GuiTesting.Lib.Threading;

namespace Codice.Examples.GuiTesting.MacOS.threading
{
    public class MacApplicationTimerBuilder : IApplicationTimerBuilder
    {
        IApplicationTimer IApplicationTimerBuilder.Get(
            bool bModalMode, ThreadWaiter.TimerTick timerTickDelegate)
        {
            return new MacApplicationTimer(DEFAULT_TIMER_INTERVAL, timerTickDelegate);
        }

        const double DEFAULT_TIMER_INTERVAL = 0.0001;
    }

    public class MacApplicationTimer : IApplicationTimer
    {
        internal MacApplicationTimer(
            double timerInterval, ThreadWaiter.TimerTick timerTickDelegate)
        {
            mTimerInterval = timerInterval;
            mTimerTickDelegate = timerTickDelegate;
        }

        void IApplicationTimer.Start()
        {
            mTimer = NSTimer.CreateRepeatingScheduledTimer(
                mTimerInterval, OnTimerTick);
            NSRunLoop.Current.AddTimer(mTimer, NSRunLoopMode.EventTracking);
        }

        void IApplicationTimer.Stop()
        {
            if (mTimer == null)
                return;

            mTimer.Invalidate();
            mTimer = null;
        }

        void OnTimerTick(NSTimer timer)
        {
            mTimerTickDelegate();
        }

        readonly double mTimerInterval;
        readonly ThreadWaiter.TimerTick mTimerTickDelegate;

        NSTimer mTimer;
    }
}
