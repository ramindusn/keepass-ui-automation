using System;
using System.IO;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Setup
{
    // The settings every UI test runs with. Change a value here and nothing else needs to change.
    public static class TestConfig
    {
        // Folder for what the tests leave behind, under the test run's working directory.
        public const string OutputDir = "artifacts";

        public static class Timeouts
        {
            // Finding a control or a window. Every wait in the framework polls up to this long.
            public static readonly TimeSpan Element = TimeSpan.FromSeconds(10);

            // The main window appearing after launch.
            public static readonly TimeSpan Launch = TimeSpan.FromSeconds(30);

            // The app coming to the front, so that clicks reach it.
            public static readonly TimeSpan Foreground = TimeSpan.FromSeconds(10);

            // The recorder releasing the video file after the test.
            public static readonly TimeSpan VideoFinish = TimeSpan.FromSeconds(15);
        }

        public static class Capture
        {
            // Off, On (every test), or RetainOnFailure (recorded, kept only when the test fails).
            public static readonly VideoMode Video = VideoMode.On;

            // A screenshot of the screen as it was when a test failed.
            public static readonly bool ScreenshotOnFailure = true;

            // The UIA tree of the app's window as it was when a test failed, and how deep it goes.
            public static readonly bool UiaTreeOnFailure = true;
            public static readonly int UiaTreeDepth = 8;
        }

        // ---- Derived from the environment; not meant to be edited ----

        // Set by GitHub Actions. On CI, evidence that cannot be produced fails the test;
        // locally it is only a warning, so the suite runs without ffmpeg.
        public static readonly bool IsCi = string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase);

        public static readonly string ArtifactsDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, OutputDir);
    }

    public enum VideoMode
    {
        Off,
        On,
        RetainOnFailure
    }
}
