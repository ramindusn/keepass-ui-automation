using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Add Entry / Edit Entry / View Entry (PwEntryForm): one form under three titles.
    public sealed class EntryDialog : ScreenObject
    {
        public const string ViewerTitle = "View Entry (Read-Only)";

        // The history list opens with fixed "Dialog (unsaved)" and "Current" rows; versions follow, newest first.
        public const int NewestEarlierVersionRow = 2;

        public EntryDialog(Window window) : base(window)
        {
            Window = window;
        }

        public Window Window { get; }

        public string Title
        {
            get { return Find("m_tbTitle").AsTextBox().Text; }
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

        public void ClickView()
        {
            Find("m_btnHistoryView").Click();
        }

        // The selected history row opens read-only in another copy of this form.
        public EntryDialog WaitForViewer()
        {
            return new EntryDialog(Waits.ForModalWindow(Window, ViewerTitle));
        }

        public void ClickOk()
        {
            Find("m_btnOK").Click();
        }

        public void ClickCancel()
        {
            Find("m_btnCancel").Click();
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
