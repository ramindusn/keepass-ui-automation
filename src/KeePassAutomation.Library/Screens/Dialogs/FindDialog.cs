using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Find > Find... (SearchForm). Results replace the contents of the entry list.
    public sealed class FindDialog : ScreenObject
    {
        private readonly Element _searchText;
        private readonly Element _ok;

        public FindDialog(AppSession session) : base(session, "Find")
        {
            _searchText = ById("m_tbSearch");
            _ok = ById("m_btnOK");
        }

        public void SetSearchText(string text)
        {
            _searchText.SetText(text);
        }

        public void ClickOk()
        {
            _ok.Click();
        }
    }
}
