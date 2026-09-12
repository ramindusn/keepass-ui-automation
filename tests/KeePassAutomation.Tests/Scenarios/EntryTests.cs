using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class EntryTests : DatabaseTestBase
    {
        private EntryDialog _entryDialog;
        private EntryViewer _entryViewer;

        [SetUp]
        public void CreatePages()
        {
            _entryDialog = new EntryDialog(Session);
            _entryViewer = new EntryViewer(Session);
        }

        [Test]
        [Requirement("REQ-004", "Changes to an entry are kept in its history")]
        public void EditedEntryKeepsItsPreviousVersion()
        {
            CreateSavedDatabase();

            // The toolbar, not the Entry menu: KeePass rebuilds that menu as it opens, and its items keep
            // reporting the names they had before, so "Add Entry..." is not findable by name.
            MainWindow.ClickToolbarButton("Add Entry");
            _entryDialog.SetTitle("Before");
            _entryDialog.ClickOk();

            // Selected and opened with Enter: a double-click lands mid-row, where the column under the
            // pointer decides what happens.
            MainWindow.SelectEntry("Before");
            MainWindow.PressEnter();
            _entryDialog.SetTitle("After");
            _entryDialog.ClickOk();

            MainWindow.SelectEntry("After");
            MainWindow.PressEnter();
            _entryDialog.SelectTab("History");
            _entryDialog.SelectHistoryRow(EntryDialog.NewestEarlierVersionRow);
            _entryDialog.ClickView();

            var previousTitle = _entryViewer.EntryTitle;
            _entryViewer.ClickCancel();
            _entryDialog.ClickCancel();

            Assert.That(previousTitle, Is.EqualTo("Before"));
        }
    }
}
