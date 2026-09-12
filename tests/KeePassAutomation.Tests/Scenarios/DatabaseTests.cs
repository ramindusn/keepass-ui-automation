using KeePassAutomation.Screens;
using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class DatabaseTests : DatabaseTestBase
    {
        private MainWindow _mainWindow;
        private FileDialog _fileDialog;
        private OpenDatabaseDialog _openDatabaseDialog;

        [SetUp]
        public void BeforeEach()
        {
            _mainWindow = new MainWindow(Session);
            _fileDialog = new FileDialog(Session);
            _openDatabaseDialog = new OpenDatabaseDialog(Session);
        }

        [Test]
        [Requirement("REQ-002", "A database opens only with the correct master key")]
        public void OpensWithTheCorrectMasterKey()
        {
            CreateSavedDatabase();
            _mainWindow.ClickMenuItem("File", "Close");
            _mainWindow.WaitForDatabaseToClose();

            _mainWindow.ClickToolbarButton("Open Database");
            _fileDialog.TypeFileName(DatabasePath);
            _fileDialog.PressEnter();
            _openDatabaseDialog.TypePassword(MasterPassword);
            _openDatabaseDialog.ClickOk();
            _mainWindow.WaitForDatabaseToOpen();

            Assert.That(_mainWindow.IsDatabaseOpen, Is.True);
        }
    }
}
