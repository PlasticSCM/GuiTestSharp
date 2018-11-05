using System;
using System.Threading;

namespace Codice.Examples.GuiTesting.Lib.Threading
{
    public class ApplicationThread
    {
        public bool IsRunning { get { lock (mLock) { return mbIsRunning; } } }
        public Exception Exception { get { lock (mLock) { return mException; } } }

        public delegate void Operation();

        public ApplicationThread(Operation performOperationDelegate)
        {
            SetRunning(true);
            mPerformOperationDelegate = performOperationDelegate;
        }

        public void Execute()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadWork));
        }

        void ThreadWork(object state)
        {
            SetException(null);

            try
            {
                mPerformOperationDelegate();
            }
            catch (Exception ex)
            {
                SetException(
                    ex.InnerException == null
                    ? ex
                    : ex.InnerException);
            }
            finally
            {
                SetRunning(false);
            }
        }

        void SetRunning(bool bIsRunning)
        {
            lock (mLock)
            {
                mbIsRunning = bIsRunning;
            }
        }

        void SetException(Exception exception)
        {
            lock (mLock)
            {
                mException = exception;
            }
        }

        Operation mPerformOperationDelegate;

        bool mbIsRunning;
        Exception mException;
        object mLock = new object();
    }
}