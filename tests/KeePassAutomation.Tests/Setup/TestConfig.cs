using System;
using System.IO;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Setup
{
    // The settings every UI test runs with. Nothing here knows which app is under test.
    public static class TestConfig
    {
        // ---- Where files go ----------------------------------------------------------------

        // Videos, screenshots and UIA trees, per test run. Uploaded by CI and attached in Allure.
        public static readonly string ArtifactsDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts");

        // ---- Timeouts ----------------------------------------------------------------------

        // Finding a control or a window. Every wait in the framework polls up to this long.
        public static readonly TimeSpan ElementTimeout = TimeSpan.FromSeconds(10);

        // The main window appearing after launch.
        public static readonly TimeSpan LaunchTimeout = TimeSpan.FromSeconds(30);

        // The app coming to the front, so that clicks reach it.
        public static readonly TimeSpan ForegroundTimeout = TimeSpan.FromSeconds(10);

        // Closing cleanly, before the process is killed.
        public static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(5);

        // The recorder releasing the video file after the test.
        public static readonly TimeSpan VideoFinishTimeout = TimeSpan.FromSeconds(15);

        // ---- Evidence ----------------------------------------------------------------------

        public static readonly VideoMode Video = VideoMode.On;

        // A screenshot and a UIA tree of the app as it was when a test failed.
        public static readonly bool EvidenceOnFailure = true;

        // How deep the UIA tree of a failure goes.
        public static readonly int UiaTreeDepth = 8;

        // ---- Environment -------------------------------------------------------------------

        // Set by GitHub Actions. On CI, evidence that cannot be produced fails the test;
        // locally it is only a warning, so the suite runs without ffmpeg.
        public static readonly bool IsCi = string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase);
    }

    public enum VideoMode
    {
        Off,

        // Every test leaves a video, named <Test>.Passed.mp4 or <Test>.Failed.mp4.
        On,

        // Every test is recorded, but the video of a passed test is deleted.
        RetainOnFailure
    }
}
