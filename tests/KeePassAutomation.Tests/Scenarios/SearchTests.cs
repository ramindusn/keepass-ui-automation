using KeePassAutomation.Screens;
using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.TestData;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class SearchTests : BaseTest
    {
        private MainWindow _mainWindow;
        private FindDialog _findDialog;

        private DatabaseTestData _database;

        [SetUp]
        public void BeforeEach()
        {
            _mainWindow = new MainWindow(Session);
            _findDialog = new FindDialog(Session);

            _database = new DatabaseTestData(Session);
            _database.CreateSavedDatabase();
        }

        [TearDown]
        public void AfterEach()
        {
            _database.Delete();
        }

        [Test]
        [Requirement("REQ-005", "Searching by title finds the matching entry")]
        public void SearchFindsTheMatchingEntry()
        {
            // Search for "#2", which appears only in the title of "Sample Entry #2".
            _mainWindow.ClickMenuItem("Find", "Find...");
            _findDialog.SetSearchText("#2");
            _findDialog.ClickOk();

            // Wait for the results to replace the entry list.
            _mainWindow.WaitForEntryRow("Sample Entry #2");

            Assert.That(_mainWindow.EntryTitles, Is.EquivalentTo(new[] { "Sample Entry #2" }));
        }
    }
}
