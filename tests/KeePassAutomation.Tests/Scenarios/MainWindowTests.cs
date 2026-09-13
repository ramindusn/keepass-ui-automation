using KeePassAutomation.Screens;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class MainWindowTests : BaseTest
    {
        private MainWindow _mainWindow;

        [SetUp]
        public void BeforeEach()
        {
            _mainWindow = new MainWindow(Session);
        }

        [Test]
        [Category("Smoke")]
        [Requirement("REQ-001", "The application starts and presents its main window")]
        public void ShowsTheMainWindowOnLaunch()
        {
            Assert.That(_mainWindow.Title, Does.Contain("KeePass"));
        }

        [Test]
        [Explicit("An example of an explicit test: it runs only when selected by name.")]
        public void StartsWithNoDatabaseOpen()
        {
            Assert.That(_mainWindow.IsDatabaseOpen, Is.False);
        }
    }
}
