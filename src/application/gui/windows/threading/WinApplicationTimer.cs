using System;
using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib.Threading;

namespace Codice.Examples.GuiTesting.Windows.Threading
{
    public class WinPlasticTimerBuilder : IApplicationTimerBuilder
    {
        IApplicationTimer IApplicationTimerBuilder.Get(
            bool bModalMode, ThreadWaiter.TimerTick timerTickDelegate)
        {
            return new WinPlasticTimer(DEFAULT_TIMER_INTERVAL, timerTickDelegate);
        }

        const int DEFAULT_TIMER_INTERVAL = 100;
    }

    public class WinPlasticTimer : IApplicationTimer
    {
        public WinPlasticTimer(int timerInterval, ThreadWaiter.TimerTick timerTickDelegate)
        {
            mTimerInterval = timerInterval;
            mTimerTickDelegate = timerTickDelegate;
        }

        public void Start()
        {
            mTimer = new Timer();
            mTimer.Interval = mTimerInterval;
            mTimer.Tick += OnTimerTick;

            mTimer.Start();
        }

        public void Stop()
        {
            mTimer.Stop();
            mTimer.Tick -= OnTimerTick;
            mTimer.Dispose();
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            mTimerTickDelegate();
        }

        Timer mTimer;

        readonly int mTimerInterval;
        readonly ThreadWaiter.TimerTick mTimerTickDelegate;
    }
}