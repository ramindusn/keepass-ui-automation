using KeePassAutomation.Screens;
using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.TestData;
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
        [Requirement("Changes to an entry are kept in its history", 19)]
        public void EditedEntryKeepsItsPreviousVersion()
        {
            // Add an entry called "Before".
            _mainWindow.ClickToolbarButton("Add Entry");
            _entryDialog.SetTitle("Before");
            _entryDialog.ClickOk();

            // Rename the entry to "After".
            _mainWindow.SelectEntry("Before");
            _mainWindow.PressEnter();
            _entryDialog.SetTitle("After");
            _entryDialog.ClickOk();

            // Open it again and view the newest earlier version from its history.
            _mainWindow.SelectEntry("After");
            _mainWindow.PressEnter();
            _entryDialog.SelectTab("History");
            _entryDialog.SelectHistoryRow(EntryDialog.NewestEarlierVersionRow);
            _entryDialog.ClickView();

            // Read that version's title, then close both dialogs.
            var previousTitle = _entryViewer.EntryTitle;
            _entryViewer.ClickCancel();
            _entryDialog.ClickCancel();

            Assert.That(previousTitle, Is.EqualTo("Before"));
        }
    }
}
