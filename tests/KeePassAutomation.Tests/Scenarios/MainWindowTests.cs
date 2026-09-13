using KeePassAutomation.Screens;
using KeePassAutomation.Tests.Setup;
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
        [Requirement("The application starts and presents its main window", 16)]
        public void ShowsTheMainWindowOnLaunch()
        {
            Assert.That(_mainWindow.Title, Does.Contain("KeePass"));
        }

        // Fails on purpose, so the report on main shows a failure with its screenshot, UIA tree and video.
        [Test]
        [Requirement("The application starts and presents its main window", 16)]
        public void FailsOnPurpose()
        {
            Assert.That(_mainWindow.Title, Does.Contain("Notepad"));
        }

        [Test]
        [Explicit("An example of an explicit test: it runs only when selected by name.")]
        public void StartsWithNoDatabaseOpen()
        {
            Assert.That(_mainWindow.IsDatabaseOpen, Is.False);
        }
    }
}
