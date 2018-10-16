using System;

namespace PNUnit.Agent
{
    internal class Shell
    {
        internal void Run(PNUnitAgent.TestCounter testCounter)
        {
            string line;
            while ((line = Console.ReadLine()) != "")
            {
                switch (line)
                {
                    case "help":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("\tgc: run the garbage collector");
                        Console.WriteLine("\tcollect: run a collect");
                        Console.WriteLine("\ttestcount: shows the number of tests launched");
                        Console.WriteLine("\tgcinfo: info about the garbage collector");
                        Console.WriteLine("\tprocinfo: info about the process");
                        Console.WriteLine("\tdisableconsole: turns off test output"+
                            " on the console (tests continues executing)");
                        Console.WriteLine("\tenableconsole: turns on test"+
                            " output on the console (default)");
                        Console.WriteLine("\tdisableoutput: turns off saving "+
                            "test output to a buffer to be sent back to the"+
                            " launcher (saves memory)");
                        Console.WriteLine("\tenableoutput: turns on saving" +
                            " output to a buffer to be returned to the launcher"+
                            " (default)");
                        break;
                    case "gc":
                        Console.WriteLine("Cleaning up memory {0} Mb",
                            GC.GetTotalMemory(true) / 1024 / 1024);
                        break;
                    case "collect":
                        Console.WriteLine("Collecting memory {0} Mb",
                            GC.GetTotalMemory(false) / 1024 / 1024);
                        GC.Collect();
                        Console.WriteLine("Memory collected {0} Mb",
                            GC.GetTotalMemory(false) / 1024 / 1024);
                        break;
                    case "testcount":
                        Console.WriteLine("{0} tests launched", testCounter.Get());
                        break;
                    case "gcinfo":
                        #if NET_2_0
                            Console.WriteLine("{0, -10}\t{1,16}", "Generation", "Collection count");
                            Console.WriteLine("==========\t================");
                            Console.WriteLine("{0, -10}\t{1,16}", 0, GC.CollectionCount(0));
                            Console.WriteLine("{0, -10}\t{1,16}", 1, GC.CollectionCount(1));
                            Console.WriteLine("{0, -10}\t{1,16}", 2, GC.CollectionCount(2));
                        #endif
                        Console.WriteLine("\nTotal memory {0:0,0.00} Mb",
                            (float)GC.GetTotalMemory(false) / 1024f / 1024f);
                        break;
                    case "procinfo":
                        System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();

                        Console.WriteLine("{0, -25}\t{1,20}", "Entry", "Value");
                        Console.WriteLine("========================\t=======================");
                        Console.WriteLine("{0, -25}\t{1,20}", "Proc Id", p.Id);
                        Console.WriteLine("{0, -25}\t{1,20}", "Handle count", p.HandleCount);
                        Console.WriteLine("{0, -25}\t{1,20}", "Thread count", p.Threads.Count);
                        #if NET_2_0
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Non paged system mem",
                                GetMb(p.NonpagedSystemMemorySize64));
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Paged mem size",
                                GetMb(p.PagedMemorySize64));
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Paged system mem size",
                                GetMb(p.PagedSystemMemorySize64));
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Peak paged mem size",
                                GetMb(p.PeakPagedMemorySize64));
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Peak virtual mem size",
                                GetMb(p.PeakVirtualMemorySize64));
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Peak working set",
                                GetMb(p.PeakWorkingSet64));
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Private mem size",
                                GetMb(p.PrivateMemorySize64));
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Virtual mem size",
                                GetMb(p.VirtualMemorySize64));
                            Console.WriteLine("{0, -25}\t{1,20:0,0.00} Mb", "Working set",
                                GetMb(p.WorkingSet64));
                        #endif
                        Console.WriteLine("{0, -25}\t{1,20}", "Start time", p.StartTime);
                        Console.WriteLine("{0, -25}\t{1,20}", "Privileged proc time", p.PrivilegedProcessorTime);
                        Console.WriteLine("{0, -25}\t{1,20}", "User proc time", p.UserProcessorTime);
                        Console.WriteLine("{0, -25}\t{1,20}", "Total proc time", p.TotalProcessorTime);
                        p.Close();
                        break;
                    case "disableconsole":
                        TestConsoleAccess.DisableConsole();
                        break;
                    case "enableconsole":
                        TestConsoleAccess.EnableConsole();
                        break;
                    case "disableoutput":
                        TestConsoleAccess.DisableStoreOutput();
                        break;
                    case "enableoutput":
                        TestConsoleAccess.EnableStoreOutput();
                        break;
                }
            }
        }

        private float GetMb(long val)
        {
            return (float)val / 1024f / 1024f;
        }
    }
}