using System;
using System.IO;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Setup
{
    // All the settings the tests run with.
    public static class TestConfig
    {
        // The folder for videos, screenshots and UI trees.
        public const string OutputDir = "artifacts";

        public static class Timeouts
        {
            // How long to wait for a control or a window.
            public static readonly TimeSpan Element = TimeSpan.FromSeconds(10);

            // How long to wait for KeePass to start.
            public static readonly TimeSpan Launch = TimeSpan.FromSeconds(30);

            // How long to wait for the video file to be saved.
            public static readonly TimeSpan VideoFinish = TimeSpan.FromSeconds(15);
        }

        public static class Capture
        {
            // Off, On for every test, or RetainOnFailure to keep only failed tests' videos.
            public static readonly VideoMode Video = VideoMode.On;

            // Take a screenshot when a test fails.
            public static readonly bool ScreenshotOnFailure = true;

            // Save the UI tree when a test fails.
            public static readonly bool UiaTreeOnFailure = true;

            // How many levels of the UI tree to save.
            public static readonly int UiaTreeDepth = 8;
        }

        // True on GitHub Actions, where missing evidence fails the test.
        public static readonly bool IsCi = string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase);

        // The full path of the output folder.
        public static readonly string ArtifactsDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, OutputDir);
    }

    public enum VideoMode
    {
        Off,
        On,
        RetainOnFailure
    }
}
