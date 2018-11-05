using System;

namespace PNUnit.Launcher
{
    [Serializable]
    public class TestRange
    {
        public int StartTest = 0;
        public int EndTest = 0;

        public static TestRange FromString(string range)
        {
            if (string.IsNullOrEmpty(range))
                return null;

            // format 0-15 or 80-
            string[] parts = range.Split('-');

            if (parts.Length < 1)
                return null;

            int ini;

            if (!int.TryParse(parts[0], out ini))
                return null;

            if (parts[1] == string.Empty ||
                parts[1].Equals("LAST", StringComparison.InvariantCultureIgnoreCase))
                return new TestRange(ini, LAST);

            int end;

            if (!int.TryParse(parts[1], out end))
                return null;

            return new TestRange(ini, end);
        }

        public TestRange(int ini, int end)
        {
            SetTestRange(ini, end);
        }

        public void SetTestRange(int ini, int end)
        {
            StartTest = ini;
            EndTest = end;
        }

        public void ResetTestRange()
        {
            SetTestRange(0, 0);
        }

        public override string ToString()
        {
            string end = (EndTest == LAST) ? "LAST" : string.Format("{0}", EndTest);
            return string.Format("{0}-{1}", StartTest, end);
        }

        public const int LAST = int.MaxValue;
    }
}