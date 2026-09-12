using KeePassAutomation.Framework.Diagnostics;
using KeePassAutomation.Screens;
using KeePassAutomation.Tests.Setup;
using KeePassAutomation.Tests.Traceability;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Scenarios
{
    public class SmokeTests : KeePassTestBase
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
        [Explicit("Diagnostic: writes the main window's UIA tree to artifacts/ for finding locators.")]
        public void DumpUiaTree()
        {
            var path = UiaTreeDump.WriteTo(ArtifactsDirectory, "main-window.tree.txt", Session.MainWindow, 12);

            TestContext.AddTestAttachment(path, "Full UIA tree");
            TestContext.WriteLine("UIA tree written to " + path);
        }
    }
}
