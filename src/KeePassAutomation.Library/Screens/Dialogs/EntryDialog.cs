using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Add Entry / Edit Entry (PwEntryForm).
    public sealed class EntryDialog : ScreenObject
    {
        public const string AddTitle = "Add Entry";
        public const string EditTitle = "Edit Entry";

        // The history list opens with fixed "Dialog (unsaved)" and "Current" rows; versions follow, newest first.
        private const int NewestEarlierVersion = 2;

        public EntryDialog(Window window) : base(window)
        {
            Window = window;
        }

        public Window Window { get; }

        public EntryDialog SetTitle(string title)
        {
            Find("m_tbTitle").AsTextBox().Text = title;
            return this;
        }

        public void Save()
        {
            Find("m_btnOK").Click();
        }

        public void Cancel()
        {
            Find("m_btnCancel").Click();
        }

        // Opens the newest earlier version from the History tab and returns its title.
        public string PreviousVersionTitle()
        {
            SelectTab("History");
            HistoryRowAt(NewestEarlierVersion).Click();

            Find("m_btnHistoryView").Click();
            var viewer = Waits.ForModalWindow(Window, "View Entry (Read-Only)");
            var title = Waits.ForDescendant(viewer, "m_tbTitle").AsTextBox().Text;
            Waits.ForDescendant(viewer, "m_btnCancel").Click();

            return title;
        }

        // Tabs are named by their caption, not by the name of the page behind them.
        private void SelectTab(string caption)
        {
            FindByName(Find("m_tabMain"), ControlType.TabItem, caption).AsTabItem().Select();
        }

        private AutomationElement HistoryRowAt(int index)
        {
            return Waits.For(Root, () => HistoryRowOrNull(index), "row " + (index + 1) + " of the history list");
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
