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
        private FileDialog _fileDialog;
        private OpenDatabaseDialog _openDatabaseDialog;
        private DatabaseTestData _database;

        [SetUp]
        public void BeforeEach()
        {
            _mainWindow = new MainWindow(Session);
            _fileDialog = new FileDialog(Session);
            _openDatabaseDialog = new OpenDatabaseDialog(Session);

            _database = new DatabaseTestData(Session);
            _database.CreateSavedDatabase();
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
            _mainWindow.ClickMenuItem("File", "Close");
            _mainWindow.WaitForDatabaseToClose();

            _mainWindow.ClickToolbarButton("Open Database");
            _fileDialog.TypeFileName(_database.Path);
            _fileDialog.PressEnter();
            _openDatabaseDialog.TypePassword(DatabaseTestData.MasterPassword);
            _openDatabaseDialog.ClickOk();
            _mainWindow.WaitForDatabaseToOpen();

            Assert.That(_mainWindow.IsDatabaseOpen, Is.True);
        }
    }
}
