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

#if !NETCORE
            HasSeparator = true;
#endif
        }

#if !NETCORE
        public override void Dispose()
        {
            Destroy();
            base.Dispose();
        }
#endif

        internal void AddComponents(params Widget[] widgets)
        {
            foreach (Widget widget in widgets)
            {
#if !NETCORE
                ControlPacker.Add(VBox, widget);
#else
                ControlPacker.Add(ContentArea, widget);
#endif
            }

            ShowAll();
        }

        internal Button CreateOkButton(string buttonText)
        {
            return (Button)AddButton(buttonText, ResponseType.Ok);
        }

        internal ResponseType RunModal()
        {
#if !NETCORE
            WindowHandler.SetActiveDialogForTesting(this);
#endif

            ResponseType responseType;
            while ((responseType = (ResponseType)Run()) == ResponseType.None) { }

            return responseType;
        }
    }
}
