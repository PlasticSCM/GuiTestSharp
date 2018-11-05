namespace Codice.Examples.GuiTesting.GuiTestInterfaces
{
    public interface ITesteableApplicationWindow
    {
        void ClickAddButton();
        void ClickRemoveButton();

        bool AreButtonsEnabled();

        void ChangeText(string text);
        string GetText();

        int GetItemsInListCount();
        string GetItemInListAt(int index);

        string GetProgressMessage();
        string GetErrorMessage();

        ITesteableErrorDialog GetErrorDialog();
    }
}
