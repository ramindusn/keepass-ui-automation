using System;
using Allure.NUnit;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;
using KeePassAutomation.Framework.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace KeePassAutomation.Tests.Setup
{
    // A fresh KeePass before each test, closed after it. Recording and evidence come from
    // GlobalBaseConfig, so this class only knows how to start and stop the app.
    [AllureNUnit]
    public abstract class BaseTest
    {
        // NUnit reuses one fixture instance for all its tests, so these are reset before each test.
        private AppSession _session;
        private TestRecording _recording;

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

        [SetUp]
        public void LaunchKeePass()
        {
            _session = null;
            _recording = null;

            Waits.Default = GlobalBaseConfig.ElementTimeout;

            // Started before launch, so a failed launch is on the video too.
            _recording = GlobalBaseConfig.StartRecording(TestContext.CurrentContext.Test.Name);

            _session = AppSession.Launch(GlobalBaseConfig.ArtifactsDirectory);
        }

        [TearDown]
        public void CloseKeePass()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var outcome = TestContext.CurrentContext.Result.Outcome.Status;

            // Failure evidence first, while KeePass is still on screen.
            if (_session != null && outcome == TestStatus.Failed)
            {
                GlobalBaseConfig.CaptureFailureEvidence(_session.MainWindow, testName);
            }

            var video = GlobalBaseConfig.FinishRecording(_recording);
            _recording = null;

            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }

            if (video != null)
            {
                GlobalBaseConfig.AttachVideo(video, testName, outcome);
            }
        }
    }
}
