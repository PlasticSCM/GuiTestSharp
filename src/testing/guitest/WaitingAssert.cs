using System.Threading;

using NUnit.Framework;

namespace GuiTest
{
    internal static class WaitingAssert
    {
        internal delegate bool BoolActualValueDelegate();
        internal delegate string StringActualValueDelegate();
        internal delegate int IntActualValueDelegate();
        internal delegate object ObjectActualValueDelegate();

        internal static void IsNotNull(
            ObjectActualValueDelegate actualDelegate,
            string message)
        {
            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                object result = actualDelegate();
                if (result != null)
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(message);
        }

        internal static void IsNullOrEmpty(
            StringActualValueDelegate actualDelegate,
            string message)
        {
            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                string result = actualDelegate();
                if (string.IsNullOrEmpty(result))
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(message);
        }

        internal static void IsNotNullOrEmpty(
            StringActualValueDelegate actualDelegate,
            string message)
        {
            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                string result = actualDelegate();
                if (!string.IsNullOrEmpty(result))
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(message);
        }

        internal static void StartsWith(
            string expected,
            StringActualValueDelegate actualDelegate,
            string message)
        {
            string actual = null;

            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                actual = actualDelegate();

                if (actual.StartsWith(expected))
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(
                string.Format("{0}. Expected starting with '{1}' but was '{2}'",
                          message, expected, actual));
        }

        internal static void Contains(
            string expected,
            StringActualValueDelegate actualDelegate,
            string message)
        {
            string actual = null;

            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                actual = actualDelegate();

                if (actual.Contains(expected))
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(
                string.Format("{0}. Expected contains '{1}' but was '{2}'",
                          message, expected, actual));
        }

        internal static void AreEqual(
            string expected,
            StringActualValueDelegate actualDelegate,
            string message)
        {
            string actual = null;

            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                actual = actualDelegate();

                if (actual == expected)
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(
                string.Format("{0}. Expected '{1}' but was '{2}'",
                          message, expected, actual));
        }

        internal static void AreEqual(
            int expected,
            IntActualValueDelegate actualDelegate,
            string message)
        {
            int actual;

            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                actual = actualDelegate();

                if (actual == expected)
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(
                string.Format("{0}. Expected '{1}' but was '{2}'",
                          message, expected, actual));
        }

        internal static void Greater(
            int expected,
            IntActualValueDelegate actualDelegate,
            string message)
        {
            int actual;

            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                actual = actualDelegate();

                if (actual > expected)
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(
                string.Format("{0}. Expected greater than '{1}' but was '{2}'",
                          message, expected, actual));
        }

        internal static void AreNotEqual(
            string expected,
            StringActualValueDelegate actualDelegate,
            string message)
        {
            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                string actual = actualDelegate();

                if (actual != expected)
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(
                string.Format("{0}. Expected different from '{1}' but was the same",
                          message, expected));
        }

        internal static void IsTrue(
            BoolActualValueDelegate actualDelegate,
            string message)
        {
            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                if (actualDelegate())
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(message);
        }

        internal static void IsFalse(
            BoolActualValueDelegate actualDelegate,
            string message)
        {
            int currentWait = 0;
            do
            {
                Thread.Sleep(SLEEP_INTERVAL);

                if (!actualDelegate())
                    return;

                currentWait += SLEEP_INTERVAL;
            } while (currentWait <= mMaxWaitTime);

            Assert.Fail(message);
        }

        internal static void SetMaxWaitTime(int milliseconds)
        {
            mMaxWaitTime = milliseconds;
        }

        static int mMaxWaitTime = DEFAULT_MAX_WAIT_TIME;
        const int SLEEP_INTERVAL = 100;
        const int DEFAULT_MAX_WAIT_TIME = 20000;
    }
}