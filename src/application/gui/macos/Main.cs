using AppKit;

namespace Codice.Examples.GuiTesting.MacOS
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();

            // You only need this hack if you removed your MainMenu.xib class,
            // which is in charge of pointing out which is your Application's
            // delegate.
            NSApplication.SharedApplication.Delegate = new AppDelegate();

            NSApplication.Main(args);
        }
    }
}
