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
            MainWindow.CloseDatabase();

            MainWindow.OpenDatabase(DatabasePath).Unlock(MasterPassword);
            MainWindow.WaitForDatabaseToOpen();

            Assert.That(MainWindow.IsDatabaseOpen, Is.True);
        }
    }
}
