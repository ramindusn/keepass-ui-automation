using System;
using System.IO;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Screens;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Setup
{
    // Launch and teardown, written once. A fresh KeePass per test keeps the tests independent.
    public abstract class KeePassTestBase
    {
        // NUnit reuses one fixture instance for all its tests, so this is reset before each test.
        private AppSession _session;

        protected AppSession Session
        {
            get
            {
                if (_session == null)
                {
                    throw new InvalidOperationException("KeePass is not running.");
                }

                return _session;
            }
        }

        protected MainWindow MainWindow { get; private set; }

        protected static string ArtifactsDirectory { get; } =
            Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts");

        [SetUp]
        public void LaunchKeePass()
        {
            _session = null;
            _session = AppSession.Launch();
            MainWindow = new MainWindow(_session.MainWindow);
        }

        [TearDown]
        public void CloseKeePass()
        {
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
        }
    }
}
