namespace Codice.Examples.GuiTesting.Lib.Interfaces
{
    public interface IProgressControls
    {
        void ShowProgress(string message);
        void HideProgress();

        void ShowError(string message);
    }
}
