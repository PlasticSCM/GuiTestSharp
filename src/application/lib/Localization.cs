using System.Collections.Generic;

namespace Codice.Examples.GuiTesting.Lib
{
    public class Localization
    {
        public enum Name
        {
            Ok,
            AddButton,
            RemoveButton,
            TextInputLabel,
            AddingElementProgressText,
            RemovingElementProgressText,
            ElementCantBeEmptyErrorMessage
        }

        Localization()
        {
            mTexts = new Dictionary<Name, string>();
            mTexts.Add(Name.Ok, "OK");
            mTexts.Add(Name.AddButton, "Add");
            mTexts.Add(Name.RemoveButton, "Remove");
            mTexts.Add(Name.TextInputLabel, "Text: ");
            mTexts.Add(Name.AddingElementProgressText, "Adding element {0}...");
            mTexts.Add(Name.RemovingElementProgressText, "Removing element {0}...");
            mTexts.Add(Name.ElementCantBeEmptyErrorMessage, "The element can't be empty!");
        }

        public static string GetText(Name name)
        {
            return GetInstance().TryGetString(name);
        }
    
        public static string GetText(Name name, params object[] args)
        {
            string result = GetInstance().TryGetString(name);

            return (result == UNLOCALIZED)
                ? UNLOCALIZED
                : string.Format(result, args);
        }

        static Localization GetInstance()
        {
            if (mInstance == null)
                mInstance = new Localization();

            return mInstance;
        }

        string TryGetString(Name name)
        {
            if (!mTexts.ContainsKey(name))
                return "UNLOCALIZED";

            return mTexts[name];
        }

        static Localization mInstance;

        readonly Dictionary<Name, string> mTexts;

        const string UNLOCALIZED = "UNLOCALIZED";
    }
}
