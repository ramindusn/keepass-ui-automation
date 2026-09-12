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
        private readonly Element _entryTitle;
        private readonly Element _cancel;

        public EntryViewer(AppSession session)
        {
            _session = session;
            _entryTitle = ById("m_tbTitle");
            _cancel = ById("m_btnCancel");
        }

        public string EntryTitle
        {
            get { return _entryTitle.Text; }
        }

        public void ClickCancel()
        {
            _cancel.Click();
        }

        protected override AutomationElement Locate()
        {
            return _session.WaitForWindow(Title);
        }
    }
}
