using System;
using Allure.NUnit;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;
using KeePassAutomation.Framework.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace KeePassAutomation.Tests.Setup
{
    // A fresh KeePass before each test, stopped after it, with the video and failure evidence around it.
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

            Waits.Default = TestConfig.Timeouts.Element;

            // Started before launch, so a failed launch is on the video too.
            _recording = Evidence.StartVideo(TestContext.CurrentContext.Test.Name);

            _session = AppSession.Launch(TestConfig.Timeouts.Launch);
        }

        [TearDown]
        public void CloseKeePass()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var outcome = TestContext.CurrentContext.Result.Outcome.Status;

            // Failure evidence first, while KeePass is still on screen.
            if (_session != null && outcome == TestStatus.Failed)
            {
                Evidence.CaptureFailure(_session.MainWindow, testName);
            }

            var video = Evidence.StopVideo(_recording);
            _recording = null;

            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }

            Evidence.KeepVideo(video, testName, outcome);
        }
    }
}
