using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Add Entry / Edit Entry (PwEntryForm): one form under two titles.
    public sealed class EntryDialog : ScreenObject
    {
        // The history list opens with fixed "Dialog (unsaved)" and "Current" rows; versions follow, newest first.
        public const int NewestEarlierVersionRow = 2;

        private const string Tabs = "m_tabMain";
        private const string HistoryList = "m_lvHistory";

        private static readonly string[] Titles = { "Add Entry", "Edit Entry" };

        private readonly AppSession _session;
        private readonly Element _title;
        private readonly Element _viewHistory;
        private readonly Element _ok;
        private readonly Element _cancel;

        public EntryDialog(AppSession session)
        {
            _session = session;
            _title = ById("m_tbTitle");
            _viewHistory = ById("m_btnHistoryView");
            _ok = ById("m_btnOK");
            _cancel = ById("m_btnCancel");
        }

        public void SetTitle(string title)
        {
            _title.SetText(title);
        }

        // Tabs are named by their caption, not by the name of the page behind them.
        public void SelectTab(string caption)
        {
            ByName(Tabs, ControlType.TabItem, caption).Select();
        }

        public void SelectHistoryRow(int index)
        {
            Waits.For(Root, () => HistoryRowOrNull(index), "row " + (index + 1) + " of the history list").Click();
        }

        // Opens the selected history row read-only, in the EntryViewer.
        public void ClickView()
        {
            _viewHistory.Click();
        }

        public void ClickOk()
        {
            _ok.Click();
        }

        public void ClickCancel()
        {
            _cancel.Click();
        }

        protected override AutomationElement Locate()
        {
            return _session.WaitForWindow(Titles);
        }

        private AutomationElement HistoryRowOrNull(int index)
        {
            var rows = FindRows(HistoryList);

            if (index >= rows.Length)
            {
                return null;
            }

            return rows[index];
        }
    }
}
