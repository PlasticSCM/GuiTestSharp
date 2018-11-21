using System;

using Codice.Examples.GuiTesting.Lib.Threading;

namespace Codice.Examples.GuiTesting.MacOS.threading
{
    public class MacApplicationTimerBuilder : IApplicationTimerBuilder
    {
        IApplicationTimer IApplicationTimerBuilder.Get(
            bool bModalMode, ThreadWaiter.TimerTick timerTickDelegate)
        {
            throw new NotImplementedException();
        }
    }

    public class MacApplicationTimer : IApplicationTimer
    {
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
