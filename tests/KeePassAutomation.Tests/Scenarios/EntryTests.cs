using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class EntryTests : DatabaseTestBase
    {
        [Test]
        [Requirement("REQ-004", "Changes to an entry are kept in its history")]
        public void EditedEntryKeepsItsPreviousVersion()
        {
            CreateSavedDatabase();
            MainWindow.AddEntry().SetTitle("Before").Save();

            MainWindow.OpenEntry("Before").SetTitle("After").Save();

            var entry = MainWindow.OpenEntry("After");
            var previousTitle = entry.PreviousVersionTitle();
            entry.Cancel();

            Assert.That(previousTitle, Is.EqualTo("Before"));
        }
    }
}
