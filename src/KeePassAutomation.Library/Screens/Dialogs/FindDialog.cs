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

        public FindDialog(AppSession session)
        {
            _session = session;
        }

        public void SetSearchText(string text)
        {
            Find("m_tbSearch").AsTextBox().Text = text;
        }

        public void ClickOk()
        {
            Find("m_btnOK").Click();
        }

        protected override AutomationElement Locate()
        {
            return _session.WaitForWindow(Title);
        }
    }
}
