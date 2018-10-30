using Codice.Examples.GuiTesting.Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codice.Examples.GuiTesting.Linux
{
    internal class GtkGuiMessage : GuiMessage.IGuiMessage
    {
        void GuiMessage.IGuiMessage.ShowError(string title, string message)
        {
            throw new NotImplementedException();
        }
    }
}
