using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // An earlier version of an entry, opened read-only from the History tab (PwEntryForm again).
    public sealed class EntryViewer : ScreenObject
    {
        private readonly Element _entryTitle;
        private readonly Element _cancel;

        public EntryViewer(AppSession session) : base(session, "View Entry (Read-Only)")
        {
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
    }
}
