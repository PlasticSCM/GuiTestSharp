using System;
using System.IO;
using System.Collections;

namespace PNUnit.Framework
{
    [Serializable]
    public class PNUnitServices
    {
        // To be used only by the runner
        public PNUnitServices(
            PNUnitTestInfo info,
            IPNUnitServices services,
            ITestConsoleAccess consoleAccess,
            ITestLogInfo testLogInfo)
        {
            mInfo = info;
            mServices = services;
            mConsoles = new ArrayList();
            mConsoles.Add(consoleAccess);
            mTestLogInfo = testLogInfo;
            mInstance = this;
        }

        public static bool Ready()
        {
            return mInstance != null;
        }

        public static PNUnitServices Get()
        {
            if (mInstance == null)
                throw new Exception("mInstance is null");
            return mInstance;
        }

        private void CheckInfo()
        {
            if (mInfo == null)
                throw new Exception("TestInfo not initialized");
        }

        // IPNUnitServices

        public void InitBarriers()
        {
            CheckInfo();
            mServices.InitBarriers(GetTestName());
        }

        public void InitBarrier(string name, int max)
        {
            CheckInfo();
            mServices.InitBarrier(GetTestName(), name, max);
        }

        public void EnterBarrier(string barrier)
        {
            CheckInfo();
            mServices.EnterBarrier(GetTestName(), barrier);
        }

        public void SendMessage(string tag, int receivers, object message)
        {
            CheckInfo();

            WriteLine(string.Format(
                ">>>Sending message (tag:{1} receivers:{2} message:{3}) by test {0} ",
                mInfo.TestName, tag, receivers,
                message == null ? string.Empty : message.ToString()));

            int ini = Environment.TickCount;

            mServices.SendMessage(tag, receivers, message);

            WriteLine(string.Format(
                "<<<Message sent (tag:{1} receivers:{2} message:{3}) by test {0} & all receivers confirmed reception. [{4} ms]",
                mInfo.TestName, tag, receivers,
                message == null ? string.Empty : message.ToString(),
                Environment.TickCount - ini));

            if (Environment.TickCount - ini > 800)
            {
                WriteLine("WARNING!! - SendMessage is taking forever! Try to run your launcher with --iptobind to speed up the process. Most likely you have an isue with your hostname");
            }
        }

        public object ReceiveMessage(string tag)
        {
            CheckInfo();
            WriteLine(
                string.Format(">>>Receiving message (tag:{1}) by test {0}",
                mInfo.TestName, tag));
            object message = mServices.ReceiveMessage(tag);
            WriteLine(
                string.Format("<<<Received message (tag:{1} message:{2}) by test {0}",
                mInfo.TestName, tag, message.ToString()));
            return message;
        }

        public void ISendMessage(string tag, int receivers, object message)
        {
            CheckInfo();

            WriteLine(string.Format(
                ">>>Sending msg (tag:{1} message:{2}) by test {0} ",
                mInfo.TestName, tag,
                message == null ? string.Empty : message.ToString()));

            int ini = Environment.TickCount;

            mServices.ISendMessage(tag, receivers, message);

            WriteLine(string.Format(
                "<<<Message sent (tag:{1} message:{2}) by test {0} & all receivers confirmed reception.  [{3} ms]",
                mInfo.TestName, tag,
                message == null ? string.Empty : message.ToString(),
                Environment.TickCount - ini));

            if (Environment.TickCount - ini > 800)
            {
                WriteLine("WARNING!! - SendMessage is taking forever! Try to run your launcher with --iptobind to speed up the process. Most likely you have an isue with your hostname");
            }
        }

        public object IReceiveMessage(string tag)
        {
            CheckInfo();
            WriteLine(
                string.Format(">>>Looking for message (tag:{1}) by test {0}",
                mInfo.TestName, tag));
            object msg = mServices.IReceiveMessage(tag);
            WriteLine(
                string.Format("<<<Search for message (tag{1}) by test {0}",
                mInfo.TestName, tag));
            return msg;
        }

        public string[] GetTestWaitBarriers()
        {
            CheckInfo();
            return mInfo.WaitBarriers;
        }

        public string GetTestName()
        {
            CheckInfo();
            return mInfo.TestName;
        }

        public string[] GetTestParams()
        {
            CheckInfo();
            return mInfo.TestParams;
        }

        public void SetInfoOSVersion(string value)
        {
            mTestLogInfo.SetOSVersion(value);
        }

        public void SetInfoBackendType(string value)
        {
            mTestLogInfo.SetBackendType(value);
        }

        public void WriteLine(string s)
        {
            if( mConsoles == null )
                throw new Exception("mConsoles is null");

            lock (mConsoles.SyncRoot)
            {
                foreach (ITestConsoleAccess console in mConsoles)
                {
                    console.WriteLine(s);
                }
            }
        }

        public void WriteLine(string s, params object[] args)
        {
            WriteLine(string.Format(s, args));
        }

        public void Write(char[] buf)
        {
            if (mConsoles == null)
                throw new Exception("mConsoles is null");

            lock (mConsoles.SyncRoot)
            {
                foreach (ITestConsoleAccess console in mConsoles)
                {
                    console.Write(buf);
                }
            }
        }

        public void ClearConsoles()
        {
            if (mConsoles == null)
                throw new Exception("mConsoles is null");

            lock (mConsoles.SyncRoot)
            {
                foreach (ITestConsoleAccess console in mConsoles)
                {
                    console.Clear();
                }
            }
        }

        public void RegisterConsole(ITestConsoleAccess console)
        {
            lock (mConsoles)
            {
                if (!mConsoles.Contains(console))
                    mConsoles.Add(console);
            }
        }

        public void UnregisterConsole(ITestConsoleAccess console)
        {
            lock (mConsoles)
            {
                mConsoles.Remove(console);
            }
        }

        public string GetTestStartBarrier()
        {
            CheckInfo();
            if (mInfo.StartBarrier == null || mInfo.StartBarrier == string.Empty)
                mInfo.StartBarrier = Names.ServerBarrier;
            return mInfo.StartBarrier;
        }

        public string GetTestEndBarrier()
        {
            CheckInfo();
            if (mInfo.EndBarrier == null || mInfo.EndBarrier == string.Empty)
                mInfo.EndBarrier = Names.EndBarrier;
            return mInfo.EndBarrier;
        }

        public string GetUserValue(string key)
        {
            if( mInfo == null || mInfo.UserValues == null || mInfo.UserValues.Count == 0)
                return null;

            return (string) mInfo.UserValues[key];
        }

        public static IPNUnitServices GetPNunitServicesProxy(string server)
        {
            // this server is the launcher host:port

            IPNUnitServices result = (IPNUnitServices)Activator.GetObject(
                typeof(IPNUnitServices),
                string.Format("tcp://{0}/{1}", server, PNUNIT_SERVICES_NAME));

            return result;
        }

        public const string PNUNIT_SERVICES_NAME = "IPNUnitServices";

        PNUnitTestInfo mInfo = null;
        IPNUnitServices mServices = null;
        ArrayList mConsoles = null;
        ITestLogInfo mTestLogInfo = null;
        static PNUnitServices mInstance = null;
    }
}
