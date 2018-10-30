using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codice.Examples.GuiTesting.Linux
{
    internal class ApplicationWindow : Gtk.Window
    {
        internal ApplicationWindow() : base(Gtk.WindowType.Toplevel)
        {
            InitializeWindow();
            BuildComponents();
        }

        void InitializeWindow()
        {

        }

        void BuildComponents()
        {

        }
    }
}
