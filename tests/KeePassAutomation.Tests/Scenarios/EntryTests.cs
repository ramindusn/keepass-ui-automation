using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class EntryTests : DatabaseTestBase
    {
        [Test]
        [Requirement("REQ-004", "Changes to an entry are kept in its history")]
        public void EditedEntryKeepsItsPreviousVersion()
        {
            CreateSavedDatabase();

            // The toolbar, not the Entry menu: KeePass rebuilds that menu as it opens, and its items keep
            // reporting the names they had before, so "Add Entry..." is not findable by name.
            MainWindow.ClickToolbarButton("Add Entry");
            EntryDialog.SetTitle("Before");
            EntryDialog.ClickOk();

            // Selected and opened with Enter: a double-click lands mid-row, where the column under the
            // pointer decides what happens.
            MainWindow.SelectEntry("Before");
            MainWindow.PressEnter();
            EntryDialog.SetTitle("After");
            EntryDialog.ClickOk();

            MainWindow.SelectEntry("After");
            MainWindow.PressEnter();
            EntryDialog.SelectTab("History");
            EntryDialog.SelectHistoryRow(EntryDialog.NewestEarlierVersionRow);
            EntryDialog.ClickView();

            var previousTitle = EntryViewer.EntryTitle;
            EntryViewer.ClickCancel();
            EntryDialog.ClickCancel();

            Assert.That(previousTitle, Is.EqualTo("Before"));
        }
    }
}
