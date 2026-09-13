using KeePassAutomation.Screens;
using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.TestData;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class DatabaseTests : BaseTest
    {
        private MainWindow _mainWindow;
        private OpenFileDialog _openFileDialog;
        private OpenDatabaseDialog _openDatabaseDialog;
        private DatabaseTestData _database;

        [SetUp]
        public void BeforeEach()
        {
            _mainWindow = new MainWindow(Session);
            _openFileDialog = new OpenFileDialog(Session);
            _openDatabaseDialog = new OpenDatabaseDialog(Session);

            _database = new DatabaseTestData(Session);
            _database.CreateSavedDatabase();
            _database.CloseDatabase();
        }

        [TearDown]
        public void AfterEach()
        {
            _database.Delete();
        }

        [Test]
        [Category("Smoke")]
        [Requirement("REQ-002", "A database opens only with the correct master key")]
        public void OpensWithTheCorrectMasterKey()
        {
            // Open the database file that setup created and closed.
            _mainWindow.ClickToolbarButton("Open Database");
            _openFileDialog.TypeFileName(_database.Path);
            _openFileDialog.PressEnter();

            // Unlock it with the master password.
            _openDatabaseDialog.TypePassword(DatabaseTestData.MasterPassword);
            _openDatabaseDialog.ClickOk();
            _mainWindow.WaitForDatabaseToOpen();

            Assert.That(_mainWindow.IsDatabaseOpen, Is.True);
        }
    }
}
