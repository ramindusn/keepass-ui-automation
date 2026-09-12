using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class DatabaseTests : DatabaseTestBase
    {
        private FileDialog _fileDialog;
        private OpenDatabaseDialog _openDatabaseDialog;

        [SetUp]
        public void CreatePages()
        {
            _fileDialog = new FileDialog(Session);
            _openDatabaseDialog = new OpenDatabaseDialog(Session);
        }

        [Test]
        [Requirement("REQ-002", "A database opens only with the correct master key")]
        public void OpensWithTheCorrectMasterKey()
        {
            CreateSavedDatabase();
            MainWindow.ClickMenuItem("File", "Close");
            MainWindow.WaitForDatabaseToClose();

            MainWindow.ClickToolbarButton("Open Database");
            _fileDialog.TypeFileName(DatabasePath);
            _fileDialog.PressEnter();
            _openDatabaseDialog.TypePassword(MasterPassword);
            _openDatabaseDialog.ClickOk();
            MainWindow.WaitForDatabaseToOpen();

            Assert.That(MainWindow.IsDatabaseOpen, Is.True);
        }
    }
}
