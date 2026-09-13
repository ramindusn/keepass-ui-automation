using KeePassAutomation.Screens;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.TestData;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class EntryListTests : BaseTest
    {
        private MainWindow _mainWindow;
        private DatabaseTestData _database;

        [SetUp]
        public void BeforeEach()
        {
            _mainWindow = new MainWindow(Session);

            _database = new DatabaseTestData(Session);
            _database.CreateSavedDatabase();
        }

        [TearDown]
        public void AfterEach()
        {
            _database.Delete();
        }

        [Test]
        [Requirement("Selecting a group lists the entries it contains", 18)]
        public void SelectingAGroupListsItsEntries()
        {
            // General is one of the new database's six empty groups.
            _mainWindow.SelectGroup("General");
            var inGeneral = _mainWindow.EntryTitles;

            // The top group holds the database's two sample entries.
            _mainWindow.SelectGroup(_database.TopGroupName);
            var inTopGroup = _mainWindow.EntryTitles;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(inGeneral, Is.Empty);
                Assert.That(inTopGroup, Is.EquivalentTo(new[] { "Sample Entry", "Sample Entry #2" }));
            }
        }
    }
}
