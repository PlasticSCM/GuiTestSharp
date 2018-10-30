using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal class ProgressControls : IProgressControls
    {
        bool IProgressControls.HasError
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        void IProgressControls.HideProgress()
        {
            throw new NotImplementedException();
        }

        void IProgressControls.ShowError(string message)
        {
            throw new NotImplementedException();
        }

        void IProgressControls.ShowProgress(string message)
        {
            throw new NotImplementedException();
        }
    }
}
