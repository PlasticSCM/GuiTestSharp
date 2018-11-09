using Gtk;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal class BaseDialog : Dialog
    {
        internal BaseDialog(string title, Window parent) : base(title, parent, DialogFlags.Modal)
        {
            SetPosition(WindowPosition.CenterOnParent);
            BorderWidth = 10;
            Resizable = false;
            HasSeparator = true;
        }

        public override void Dispose()
        {
            Destroy();
            base.Dispose();
        }

        internal void AddComponents(params Widget[] widgets)
        {
            foreach (Widget widget in widgets)
                ControlPacker.Add(VBox, widget);

            ShowAll();
        }

        internal Button CreateOkButton(string buttonText)
        {
            return (Button)AddButton(buttonText, ResponseType.Ok);
        }

        internal ResponseType RunModal()
        {
            WindowHandler.SetActiveDialogForTesting(this);

            ResponseType responseType;
            while ((responseType = (ResponseType)Run()) == ResponseType.None) { }

            return responseType;
        }
    }
}
