using System.Collections.Generic;

namespace Codice.Examples.GuiTesting.Lib.Interfaces
{
    public interface IApplicationWindow
    {
        void UpdateItems(List<string> items);
        void ClearInput();
    }
}
