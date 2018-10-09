using System;
using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Threading;
using Codice.Examples.GuiTesting.Windows.Threading;

namespace Codice.Examples.GuiTesting.Windows
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationArgs appArgs = ApplicationArgs.Parse(args);

            ExceptionsHandler.SetExceptionHandlers(appArgs.IsTestingMode);

            ThreadWaiterBuilder.Initialize(new WinPlasticTimerBuilder());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ApplicationWindow(new ApplicationOperations()));
        }
    }
}
