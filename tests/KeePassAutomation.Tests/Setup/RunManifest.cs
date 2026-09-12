using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Allure.Net.Commons;
using FlaUI.UIA3;
using KeePassAutomation.Framework.AppUnderTest;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Setup
{
    // What a run has to say about itself: which build of the application it drove, on what machine,
    // from which commit, started by whom and when. A UI result means little without them, so they
    // are recorded once per run, into the report and into the evidence folder.
    public static class RunManifest
    {
        public static void Write()
        {
            var facts = Collect();

            WriteEnvironmentProperties(facts);
            WriteExecutor();
            WriteRunFile(facts);
        }

        private static Dictionary<string, string> Collect()
        {
            var executable = KeePassPackage.FindExecutable();
            var facts = new Dictionary<string, string>
            {
                { "Application", Path.GetFileName(executable) + " " + ProductVersionOf(executable) },
                { "Application.Path", executable },
                { "Application.Sha256", Sha256Of(executable) },
                { "Machine", Environment.MachineName },
                { "Machine.OperatingSystem", RuntimeInformation.OSDescription },
                { "Machine.Runtime", RuntimeInformation.FrameworkDescription },
                { "Machine.Locale", CultureInfo.CurrentCulture.Name },
                { "Machine.TimeZone", TimeZoneInfo.Local.Id },
                { "Machine.Screen", ScreenSize() },
                { "Run.StartedUtc", DateTime.UtcNow.ToString("u", CultureInfo.InvariantCulture) },
                { "Run.Commit", Variable("GITHUB_SHA") },
                { "Run.Branch", Variable("GITHUB_REF_NAME") },
                { "Run.StartedBy", Variable("GITHUB_ACTOR") },
                { "Run.Workflow", WorkflowRun() }
            };

            return facts;
        }

        // The Environment card on the report's overview page.
        private static void WriteEnvironmentProperties(Dictionary<string, string> facts)
        {
            var lines = new List<string>();

            foreach (var fact in facts)
            {
                lines.Add(fact.Key + "=" + fact.Value);
            }

            File.WriteAllLines(Path.Combine(ResultsDirectory(), "environment.properties"), lines);
        }

        // Names the run in the report, so a point on the trend links back to the run that made it.
        private static void WriteExecutor()
        {
            var runId = Variable("GITHUB_RUN_ID");

            if (runId.Length == 0)
            {
                return;
            }

            var executor = new Dictionary<string, string>
            {
                { "name", "GitHub Actions" },
                { "type", "github" },
                { "buildName", WorkflowRun() },
                { "buildOrder", Variable("GITHUB_RUN_NUMBER") },
                { "buildUrl", Variable("GITHUB_SERVER_URL") + "/" + Variable("GITHUB_REPOSITORY") + "/actions/runs/" + runId },
                { "reportName", "Allure report" },
                { "reportUrl", TestConfig.Run.ReportUrl }
            };

            var json = JsonSerializer.Serialize(executor, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(Path.Combine(ResultsDirectory(), "executor.json"), json);
        }

        // The same facts inside the evidence folder, for a reader who has the files but not the report.
        private static void WriteRunFile(Dictionary<string, string> facts)
        {
            Directory.CreateDirectory(TestConfig.ArtifactsDirectory);

            var json = JsonSerializer.Serialize(facts, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(Path.Combine(TestConfig.ArtifactsDirectory, "run.json"), json);
        }

        private static string ResultsDirectory()
        {
            var directory = AllureLifecycle.Instance.ResultsDirectory;

            Directory.CreateDirectory(directory);

            return directory;
        }

        private static string ProductVersionOf(string executable)
        {
            var version = FileVersionInfo.GetVersionInfo(executable).ProductVersion;

            if (string.IsNullOrEmpty(version))
            {
                return "unknown version";
            }

            return version;
        }

        // The build that ran, not the build that was meant to run: the hash is read off the file itself.
        private static string Sha256Of(string path)
        {
            using (var file = File.OpenRead(path))
            {
                using (var algorithm = SHA256.Create())
                {
                    var hash = algorithm.ComputeHash(file);
                    var text = new StringBuilder(hash.Length * 2);

                    foreach (var b in hash)
                    {
                        text.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                    }

                    return text.ToString();
                }
            }
        }

        // UI results depend on how much screen there was, so the desktop is measured, not assumed.
        private static string ScreenSize()
        {
            try
            {
                using (var automation = new UIA3Automation())
                {
                    var desktop = automation.GetDesktop().BoundingRectangle;

                    return desktop.Width + "x" + desktop.Height;
                }
            }
            catch (Exception ex)
            {
                return "unknown (" + ex.GetType().Name + ")";
            }
        }

        private static string WorkflowRun()
        {
            var workflow = Variable("GITHUB_WORKFLOW");
            var runId = Variable("GITHUB_RUN_ID");

            if (runId.Length == 0)
            {
                return "local run";
            }

            return workflow + " #" + runId + " attempt " + Variable("GITHUB_RUN_ATTEMPT");
        }

        // Empty off CI, which is itself the record that the run was a local one.
        private static string Variable(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);

            if (value == null)
            {
                return string.Empty;
            }

            return value;
        }
    }
}
