using System;
using System.IO;
using Allure.Net.Commons;
using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace KeePassAutomation.Tests.Setup
{
    // What a test leaves behind, as GlobalBaseConfig says: a video, and on failure a screenshot
    // and a UIA tree, all attached to the test in Allure. BaseTest calls these around each test.
    public static class Evidence
    {
        // Null when nothing is recorded: video is off, or ffmpeg is missing (a failure on CI).
        public static TestRecording StartVideo(string testName)
        {
            if (GlobalBaseConfig.Video == VideoMode.Off)
            {
                return null;
            }

            var ffmpeg = FFmpegPackage.TryFindExecutable();

            if (ffmpeg == null)
            {
                return NoVideo("ffmpeg was not found — run: pwsh tools/Get-FFmpeg.ps1");
            }

            try
            {
                return TestRecording.Start(ffmpeg, Path.Combine(GlobalBaseConfig.ArtifactsDirectory, FileNames.Sanitise(testName) + ".mp4"));
            }
            catch (Exception ex)
            {
                return NoVideo("the recording could not start: " + ex.Message);
            }
        }

        // The recorded file, or null. A recording problem must never replace the real test result.
        public static string StopVideo(TestRecording recording)
        {
            if (recording == null)
            {
                return null;
            }

            try
            {
                return recording.Finish(GlobalBaseConfig.VideoFinishTimeout);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("Could not finish the video: " + ex.Message);
                return null;
            }
        }

        // Named for its outcome and attached, or deleted when only failures are kept.
        public static void KeepVideo(string recordedPath, string testName, TestStatus outcome)
        {
            if (recordedPath == null)
            {
                return;
            }

            if (GlobalBaseConfig.Video == VideoMode.RetainOnFailure && outcome != TestStatus.Failed)
            {
                File.Delete(recordedPath);
                return;
            }

            Attach(() => RenameForOutcome(recordedPath, testName, outcome), "video");
        }

        // Called while the app is still on screen: a screenshot of a closed app tells you nothing.
        public static void CaptureFailure(AutomationElement window, string testName)
        {
            if (!GlobalBaseConfig.EvidenceOnFailure)
            {
                return;
            }

            Attach(() => Screenshots.CaptureScreen(GlobalBaseConfig.ArtifactsDirectory, testName), "screenshot");
            Attach(() => UiaTreeDump.WriteTo(GlobalBaseConfig.ArtifactsDirectory, testName + ".tree.txt", window, GlobalBaseConfig.UiaTreeDepth), "UIA tree");
        }

        private static TestRecording NoVideo(string reason)
        {
            if (GlobalBaseConfig.IsCi)
            {
                Assert.Fail("No video can be recorded: " + reason);
            }

            TestContext.WriteLine("Not recording video: " + reason);
            return null;
        }

        private static string RenameForOutcome(string recordedPath, string testName, TestStatus outcome)
        {
            var finalPath = Path.Combine(GlobalBaseConfig.ArtifactsDirectory, FileNames.Sanitise(testName) + "." + outcome + ".mp4");
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
