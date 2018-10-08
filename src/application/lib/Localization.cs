using System.Collections.Generic;

namespace Codice.Examples.GuiTesting.Lib
{
    public class Localization
    {
        public enum Name
        {
            AddButton,
            RemoveButton,
            TextInputLabel
        }

        Localization()
        {
            mTexts = new Dictionary<Name, string>();
            mTexts.Add(Name.AddButton, "Add");
            mTexts.Add(Name.RemoveButton, "Remove");
            mTexts.Add(Name.TextInputLabel, "Text: ");
        }

        public static string GetText(Name name)
        {
            return GetInstance().TryGetString(name);
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
    }
}
