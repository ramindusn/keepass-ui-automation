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

        // Fails on purpose, to show a failure with its screenshot, UIA tree and video. Remove the Ignore to run it.
        [Test]
        [Ignore("Fails on purpose; skipped until a failure needs to be shown.")]
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
