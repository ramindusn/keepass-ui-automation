using KeePassAutomation.Screens;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class EntryListTests : DatabaseTestBase
    {
        private MainWindow _mainWindow;

        [SetUp]
        public void BeforeEach()
        {
            _mainWindow = new MainWindow(Session);
        }

        [Test]
        [Requirement("REQ-003", "Selecting a group lists the entries it contains")]
        public void SelectingAGroupListsItsEntries()
        {
            // A new database has two sample entries in its top group and six empty groups under it.
            CreateSavedDatabase();

            _mainWindow.SelectGroup("General");
            var inGeneral = _mainWindow.EntryTitles;

            _mainWindow.SelectGroup(TopGroupName);
            var inTopGroup = _mainWindow.EntryTitles;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(inGeneral, Is.Empty);
                Assert.That(inTopGroup, Is.EquivalentTo(new[] { "Sample Entry", "Sample Entry #2" }));
            }
        }
    }
}
