using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Find > Find... (SearchForm). Results replace the contents of the entry list.
    public sealed class FindDialog : ScreenObject
    {
        public const string Title = "Find";

        private readonly AppSession _session;
        private readonly Element _searchText;
        private readonly Element _ok;

        public FindDialog(AppSession session)
        {
            _session = session;
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

        protected override AutomationElement Locate()
        {
            return _session.WaitForWindow(Title);
        }
    }
}
