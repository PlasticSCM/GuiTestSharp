namespace Codice.Examples.GuiTesting.Lib.Interfaces
{
    public interface IProgressControls
    {
        bool HasError { get; }

        void ShowProgress(string message);
        void HideProgress();

        void ShowError(string message);
    }
}
