using KeePassAutomation.Screens;
using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.TestData;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class EntryTests : BaseTest
    {
        private MainWindow _mainWindow;
        private EntryDialog _entryDialog;
        private EntryViewer _entryViewer;

        private DatabaseTestData _database;

        [SetUp]
        public void BeforeEach()
        {
            _mainWindow = new MainWindow(Session);
            _entryDialog = new EntryDialog(Session);
            _entryViewer = new EntryViewer(Session);

            _database = new DatabaseTestData(Session);
            _database.CreateSavedDatabase();
        }

        [TearDown]
        public void AfterEach()
        {
            _database.Delete();
        }

        [Test]
        [Requirement("REQ-004", "Changes to an entry are kept in its history")]
        public void EditedEntryKeepsItsPreviousVersion()
        {
            // The toolbar, not the Entry menu: KeePass rebuilds that menu as it opens, and its items keep
            // reporting the names they had before, so "Add Entry..." is not findable by name.
            _mainWindow.ClickToolbarButton("Add Entry");
            _entryDialog.SetTitle("Before");
            _entryDialog.ClickOk();

            // Selected and opened with Enter: a double-click lands mid-row, where the column under the
            // pointer decides what happens.
            _mainWindow.SelectEntry("Before");
            _mainWindow.PressEnter();
            _entryDialog.SetTitle("After");
            _entryDialog.ClickOk();

            _mainWindow.SelectEntry("After");
            _mainWindow.PressEnter();
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
