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

        private readonly Element _title;
        private readonly Element _tabs;
        private readonly Element _historyList;
        private readonly Element _viewHistory;
        private readonly Element _ok;
        private readonly Element _cancel;

        public EntryDialog(AppSession session) : base(session, "Add Entry", "Edit Entry")
        {
            _title = ById("m_tbTitle");
            _tabs = ById("m_tabMain");
            _historyList = ById("m_lvHistory");
            _viewHistory = ById("m_btnHistoryView");
            _ok = ById("m_btnOK");
            _cancel = ById("m_btnCancel");
        }

        public void SetTitle(string title)
        {
            _title.SetText(title);
        }

        // Tabs are named by their caption, not by the control name behind them.
        public void SelectTab(string caption)
        {
            _tabs.Child(ControlType.TabItem, caption).Select();
        }

        // Clicks a row of the history list, counting from 0.
        public void SelectHistoryRow(int index)
        {
            _historyList.Row(index).Click();
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
    }
}
