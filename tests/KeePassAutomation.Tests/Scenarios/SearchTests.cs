using KeePassAutomation.Screens.Dialogs;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class SearchTests : DatabaseTestBase
    {
        private FindDialog _findDialog;

        [SetUp]
        public void CreatePages()
        {
            _findDialog = new FindDialog(Session);
        }

        [Test]
        [Requirement("REQ-005", "Searching by title finds the matching entry")]
        public void SearchFindsTheMatchingEntry()
        {
            // A new database holds "Sample Entry" and "Sample Entry #2"; "#2" appears only in the second title.
            CreateSavedDatabase();

            MainWindow.ClickMenuItem("Find", "Find...");
            _findDialog.SetSearchText("#2");
            _findDialog.ClickOk();
            MainWindow.WaitForEntryRow("Sample Entry #2");

            Assert.That(MainWindow.EntryTitles, Is.EquivalentTo(new[] { "Sample Entry #2" }));
        }
    }
}
