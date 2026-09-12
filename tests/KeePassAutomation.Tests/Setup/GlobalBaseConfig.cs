using System;
using System.IO;
using Allure.Net.Commons;
using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace KeePassAutomation.Tests.Setup
{
    // The settings every UI test shares, and the reporting steps that use them: video of every
    // test, screenshot and UIA tree of a failure, all attached to the test in Allure. Knows no app.
    public static class GlobalBaseConfig
    {
        public static readonly TimeSpan ElementTimeout = TimeSpan.FromSeconds(10);

        public static readonly TimeSpan VideoFinishTimeout = TimeSpan.FromSeconds(15);

        public static string ArtifactsDirectory
        {
            get { return Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts"); }
        }

        public static bool RunningOnCi
        {
            get { return string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase); }
        }

        // Null when nothing can be recorded; on CI that fails the test, locally it only warns.
        public static TestRecording StartRecording(string testName)
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

        // The recorded file, or null. A recording problem must never replace the real test result.
        public static string FinishRecording(TestRecording recording)
        {
            if (recording == null)
            {
                return null;
            }

            try
            {
                return recording.Finish(VideoFinishTimeout);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("Could not finish the video: " + ex.Message);
                return null;
            }
        }

        // <Test>.Passed.mp4 or <Test>.Failed.mp4, attached to the test.
        public static void AttachVideo(string recordedPath, string testName, TestStatus outcome)
        {
            Attach(() => RenameForOutcome(recordedPath, testName, outcome), "video");
        }

        // Called while the app is still on screen: a screenshot of a closed app tells you nothing.
        public static void CaptureFailureEvidence(AutomationElement window, string testName)
        {
            Attach(() => Screenshots.CaptureScreen(ArtifactsDirectory, testName), "screenshot");
            Attach(() => UiaTreeDump.WriteTo(ArtifactsDirectory, testName + ".tree.txt", window), "UIA tree");
        }

        private static TestRecording NoRecording(string reason)
        {
            if (RunningOnCi)
            {
                Assert.Fail("No video can be recorded: " + reason);
            }

            TestContext.WriteLine("Not recording video: " + reason);
            return null;
        }

        private static string RenameForOutcome(string recordedPath, string testName, TestStatus outcome)
        {
            var finalPath = Path.Combine(ArtifactsDirectory, FileNames.Sanitise(testName) + "." + outcome + ".mp4");
            File.Move(recordedPath, finalPath, true);
            return finalPath;
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
