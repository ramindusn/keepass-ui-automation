using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Add Entry / Edit Entry (PwEntryForm): one form under two titles.
    public sealed class EntryDialog : ScreenObject
    {
        // The history list opens with fixed "Dialog (unsaved)" and "Current" rows; versions follow, newest first.
        public const int NewestEarlierVersionRow = 2;

        private static readonly string[] Titles = { "Add Entry", "Edit Entry" };

        private readonly MainWindow _owner;

        public EntryDialog(MainWindow owner)
        {
            _owner = owner;
        }

        public void SetTitle(string title)
        {
            Find("m_tbTitle").AsTextBox().Text = title;
        }

        // Tabs are named by their caption, not by the name of the page behind them.
        public void SelectTab(string caption)
        {
            FindByName(Find("m_tabMain"), ControlType.TabItem, caption).AsTabItem().Select();
        }

        public void SelectHistoryRow(int index)
        {
            Waits.For(Root, () => HistoryRowOrNull(index), "row " + (index + 1) + " of the history list").Click();
        }

        // Opens the selected history row read-only, in the EntryViewer.
        public void ClickView()
        {
            Find("m_btnHistoryView").Click();
        }

        public void ClickOk()
        {
            Find("m_btnOK").Click();
        }

        public void ClickCancel()
        {
            Find("m_btnCancel").Click();
        }

        protected override AutomationElement Locate()
        {
            return Waits.ForModalWindow(_owner.Window, Titles);
        }

        private AutomationElement HistoryRowOrNull(int index)
        {
            var rows = FindRows("m_lvHistory");

            if (index >= rows.Length)
            {
                return null;
            }

            return rows[index];
        }
    }
}
