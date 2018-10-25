using System;

namespace Codice.Examples.GuiTesting.Lib.Threading
{
    public interface IApplicationTimerBuilder
    {
        IApplicationTimer Get(
            bool bModalMode,
            ThreadWaiter.TimerTick timerTickDelegate);
    }

    public interface IApplicationTimer
    {
        void Start();
        void Stop();
    }

    public interface IThreadWaiterBuilder
    {
        IThreadWaiter GetWaiter();
        IThreadWaiter GetModalWaiter();
    }

    public interface IThreadWaiter
    {
        Exception Exception { get; }

        void Execute(
            ApplicationThread.Operation threadOperationDelegate,
            ApplicationThread.Operation afterOperationDelegate);

        void Cancel();
    }

    public static class ThreadWaiter
    {
        public delegate void TimerTick();

        public static void Initialize(IThreadWaiterBuilder threadWaiterBuilder)
        {
            mThreadWaiterBuilder = threadWaiterBuilder;
        }

        public static IThreadWaiter GetWaiter()
        {
            return mThreadWaiterBuilder.GetWaiter();
        }

        public static IThreadWaiter GetModalWaiter()
        {
            return mThreadWaiterBuilder.GetModalWaiter();
        }

        public static void Reset()
        {
            mThreadWaiterBuilder = new ThreadWaiterBuilder();
        }

        static IThreadWaiterBuilder mThreadWaiterBuilder = new ThreadWaiterBuilder();
    }

    public class ThreadWaiterBuilder : IThreadWaiterBuilder
    {
        public static void Initialize(IApplicationTimerBuilder timerBuilder)
        {
            mPlasticTimerBuilder = timerBuilder;
        }

        IThreadWaiter IThreadWaiterBuilder.GetWaiter()
        {
            return new PlasticThreadWaiter(mPlasticTimerBuilder, false);
        }

        IThreadWaiter IThreadWaiterBuilder.GetModalWaiter()
        {
            return new PlasticThreadWaiter(mPlasticTimerBuilder, true);
        }

        static IApplicationTimerBuilder mPlasticTimerBuilder;
    }

    public class PlasticThreadWaiter : IThreadWaiter
    {
        public Exception Exception { get { return mThreadOperation.Exception; } }

        internal PlasticThreadWaiter(
            IApplicationTimerBuilder timerBuilder, bool bModalMode)
        {
            mPlasticTimer = timerBuilder.Get(bModalMode, OnTimerTick);
        }

        public void Execute(
            ApplicationThread.Operation threadOperationDelegate,
            ApplicationThread.Operation afterOperationDelegate)
        {
            mThreadOperation = new ApplicationThread(threadOperationDelegate);
            mAfterOperationDelegate = afterOperationDelegate;

            mPlasticTimer.Start();

            mThreadOperation.Execute();
        }

        public void Cancel()
        {
            mbCancelled = true;
        }

        void OnTimerTick()
        {
            if (mThreadOperation.IsRunning)
                return;

            mPlasticTimer.Stop();

            if (mbCancelled)
                return;

            mAfterOperationDelegate();
        }

        bool mbCancelled = false;

        IApplicationTimer mPlasticTimer;
        ApplicationThread mThreadOperation;
        ApplicationThread.Operation mAfterOperationDelegate;
    }
}