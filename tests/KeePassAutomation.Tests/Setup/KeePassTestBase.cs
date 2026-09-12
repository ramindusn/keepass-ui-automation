using System;
using System.IO;
using Allure.Net.Commons;
using Allure.NUnit;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Diagnostics;
using KeePassAutomation.Screens;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace KeePassAutomation.Tests.Setup
{
    // Launch, video recording and teardown, written once. A failed test also leaves a screenshot and UIA tree.
    // Every test reports to Allure, which is where the evidence ends up.
    [AllureNUnit]
    public abstract class KeePassTestBase
    {
        private static readonly TimeSpan VideoFinishTimeout = TimeSpan.FromSeconds(15);

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

        protected MainWindow MainWindow { get; private set; }

        protected static string ArtifactsDirectory { get; } =
            Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts");

        private static bool RunningOnCi
        {
            get { return string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase); }
        }

        [SetUp]
        public void LaunchKeePass()
        {
            _session = null;
            _recording = null;

            // Started before launch, so a failed launch is on the video too.
            _recording = StartRecording(TestContext.CurrentContext.Test.Name);

            _session = AppSession.Launch(ArtifactsDirectory);
            MainWindow = new MainWindow(_session.MainWindow);
        }

        [TearDown]
        public void CloseKeePass()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var outcome = TestContext.CurrentContext.Result.Outcome.Status;

            // Failure evidence first, while KeePass is still on screen.
            if (_session != null && outcome == TestStatus.Failed)
            {
                CaptureFailureEvidence(_session, testName);
            }

            var video = FinishRecording();

            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }

            if (video != null)
            {
                Attach(() => RenameForOutcome(video, testName, outcome), "video");
            }
        }

        private static TestRecording StartRecording(string testName)
        {
            var ffmpeg = FFmpegPackage.TryFindExecutable();

            if (ffmpeg == null)
            {
                return NoRecording("ffmpeg was not found — run: pwsh tools/Get-FFmpeg.ps1");
            }

            try
            {
                return TestRecording.Start(ffmpeg, Path.Combine(ArtifactsDirectory, FileNames.Sanitise(testName) + ".mp4"));
            }
            catch (Exception ex)
            {
                return NoRecording("the recording could not start: " + ex.Message);
            }
        }

        // On CI a missing video is missing evidence, so it fails the test; locally the suite runs without it.
        private static TestRecording NoRecording(string reason)
        {
            if (RunningOnCi)
            {
                Assert.Fail("No video can be recorded: " + reason);
            }

            TestContext.WriteLine("Not recording video: " + reason);
            return null;
        }

        private string FinishRecording()
        {
            if (_recording == null)
            {
                return null;
            }

            // A recording problem must never replace the real test result.
            try
            {
                return _recording.Finish(VideoFinishTimeout);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("Could not finish the video: " + ex.Message);
                return null;
            }
            finally
            {
                _recording = null;
            }
        }

        private static string RenameForOutcome(string recordedPath, string testName, TestStatus outcome)
        {
            var finalPath = Path.Combine(ArtifactsDirectory, FileNames.Sanitise(testName) + "." + outcome + ".mp4");
            File.Move(recordedPath, finalPath, true);
            return finalPath;
        }

        private static void CaptureFailureEvidence(AppSession session, string testName)
        {
            Attach(() => Screenshots.CaptureScreen(ArtifactsDirectory, testName), "screenshot");
            Attach(() => UiaTreeDump.WriteTo(ArtifactsDirectory, testName + ".tree.txt", session.MainWindow), "UIA tree");
        }

        // Evidence capture must never replace the real failure with one of its own.
        private static void Attach(Func<string> capture, string description)
        {
            try
            {
                var path = capture();
                AllureApi.AddAttachment(description, MediaTypeOf(path), path);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("Could not capture " + description + ": " + ex.Message);
            }
        }

        private static string MediaTypeOf(string path)
        {
            var extension = Path.GetExtension(path);

            if (extension == ".png")
            {
                return "image/png";
            }

            if (extension == ".mp4")
            {
                return "video/mp4";
            }

            return "text/plain";
        }
    }
}
