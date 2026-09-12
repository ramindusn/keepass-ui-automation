using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // An earlier version of an entry, opened read-only from the History tab (PwEntryForm again).
    public sealed class EntryViewer : ScreenObject
    {
        public const string Title = "View Entry (Read-Only)";

        private readonly AppSession _session;

        public EntryViewer(AppSession session)
        {
            _session = session;
        }

        public string EntryTitle
        {
            get { return Find("m_tbTitle").AsTextBox().Text; }
        }

        public void ClickCancel()
        {
            Find("m_btnCancel").Click();
        }

        protected override AutomationElement Locate()
        {
            return _session.WaitForWindow(Title);
        }
    }
}
