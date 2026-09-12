using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class DatabaseTests : DatabaseTestBase
    {
        [Test]
        [Requirement("REQ-002", "A database opens only with the correct master key")]
        public void OpensWithTheCorrectMasterKey()
        {
            CreateSavedDatabase();

            MainWindow.ClickMenuItem("File", "Close");
            MainWindow.WaitForDatabaseToClose();

            MainWindow.ClickToolbarButton("Open Database");

            var open = MainWindow.WaitForFileDialog("Open Database File");
            open.TypeFileName(DatabasePath);
            open.PressEnter();

            var keyPrompt = MainWindow.WaitForOpenDatabaseDialog();
            keyPrompt.TypePassword(MasterPassword);
            keyPrompt.ClickOk();

            MainWindow.WaitForDatabaseToOpen();

            Assert.That(MainWindow.IsDatabaseOpen, Is.True);
        }
    }
}
