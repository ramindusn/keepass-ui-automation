using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // An earlier version of an entry, opened read-only from the History tab (PwEntryForm again).
    public sealed class EntryViewer : ScreenObject
    {
        public const string Title = "View Entry (Read-Only)";

        private readonly MainWindow _owner;

        public EntryViewer(MainWindow owner)
        {
            _owner = owner;
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
            return Waits.ForModalWindow(_owner.Window, Title);
        }
    }
}
