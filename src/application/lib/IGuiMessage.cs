namespace Codice.Examples.GuiTesting.Lib.Interfaces
{
    public class GuiMessage
    {
        public interface IGuiMessage
        {
            void ShowError(string title, string message);
        }

        public static void Initialize(IGuiMessage guiMessage)
        {
            mGuiMessage = guiMessage;
        }

        public static void ShowError(string title, string message)
        {
            mGuiMessage.ShowError(title, message);
        }

        static IGuiMessage mGuiMessage;
    }
}
