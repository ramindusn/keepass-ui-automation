using System;
using System.Text;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using KeePassAutomation.Framework.Diagnostics;

namespace KeePassAutomation.Framework.AppUnderTest
{
    public sealed class AppSession : IDisposable
    {
        private static readonly TimeSpan MainWindowTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan ForegroundTimeout = TimeSpan.FromSeconds(10);

        private AppSession(Application application, UIA3Automation automation, Window mainWindow)
        {
            Application = application;
            Automation = automation;
            MainWindow = mainWindow;
        }

        public Application Application { get; }

        public UIA3Automation Automation { get; }

        public Window MainWindow { get; }

        // Launch failures happen in SetUp, before TearDown has a session, so evidence is captured here.
        public static AppSession Launch(string evidenceDirectory = null)
        {
            var application = Application.Launch(KeePassPackage.FindExecutable());
            var automation = new UIA3Automation();

            // Without a timeout FlaUI waits forever, so one unexpected dialog would hang the run.
            var mainWindow = application.GetMainWindow(automation, MainWindowTimeout);

            if (mainWindow != null)
            {
                BringToFront(mainWindow, automation, application.ProcessId);
                return new AppSession(application, automation, mainWindow);
            }

            var failure = DescribeLaunchFailure(application, automation, evidenceDirectory);

            automation.Dispose();
            Kill(application);
            application.Dispose();

            throw new InvalidOperationException(failure);
        }

        public void Dispose()
        {
            ShutDownApplication();
            Automation.Dispose();
            Application.Dispose();
        }

        // Clicks are real mouse events, so they land on whichever window is in front — on CI that was the
        // runner's console, and the click never reached KeePass.
        private static void BringToFront(Window mainWindow, UIA3Automation automation, int processId)
        {
            Retry.WhileFalse(() => TryBringToFront(mainWindow, automation, processId), ForegroundTimeout);
        }

        // Windows refuses foreground to a process it did not just activate, and says so only by leaving
        // the window where it was, so the result is checked rather than assumed.
        private static bool TryBringToFront(Window mainWindow, UIA3Automation automation, int processId)
        {
            try
            {
                mainWindow.SetForeground();

                if (HasForeground(automation, processId))
                {
                    return true;
                }

                // Restoring a minimised window is an activation Windows does allow.
                var window = mainWindow.Patterns.Window.PatternOrDefault;

                if (window != null)
                {
                    window.SetWindowVisualState(WindowVisualState.Minimized);
                    window.SetWindowVisualState(WindowVisualState.Normal);
                }

                return HasForeground(automation, processId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool HasForeground(UIA3Automation automation, int processId)
        {
            return automation.FocusedElement().Properties.ProcessId.ValueOrDefault == processId;
        }

        private static string DescribeLaunchFailure(Application application, UIA3Automation automation, string evidenceDirectory)
        {
            var report = new StringBuilder()
                .AppendLine("KeePass launched but no main window appeared within "
                    + MainWindowTimeout.TotalSeconds.ToString("0") + "s.")
                .AppendLine(ProcessState(application))
                .AppendLine()
                .AppendLine("Top-level windows on the desktop (* = belongs to KeePass):");

            try
            {
                foreach (var window in automation.GetDesktop().FindAllChildren())
                {
                    var ownedByKeePass = window.Properties.ProcessId.ValueOrDefault == application.ProcessId;
                    var marker = ownedByKeePass ? "*" : " ";

                    report.AppendLine("  " + marker + " '" + window.Name + "' class='" + window.ClassName + "'");
                }
            }
            catch (Exception ex)
            {
                report.AppendLine("  <could not list windows: " + ex.GetType().Name + ">");
            }

            if (evidenceDirectory != null)
            {
                report.AppendLine()
                    .AppendLine(TrySave(() => Screenshots.CaptureScreen(evidenceDirectory, "launch-failure"), "screenshot"))
                    .AppendLine(TrySave(() => UiaTreeDump.WriteTo(evidenceDirectory, "launch-failure.desktop.txt", automation.GetDesktop(), 3), "desktop tree"));
            }

            return report.ToString();
        }

        private static string ProcessState(Application application)
        {
            try
            {
                if (application.HasExited)
                {
                    return "The KeePass process had already exited (exit code " + application.ExitCode + ").";
                }

                return "The KeePass process was still running.";
            }
            catch (Exception ex)
            {
                return "Could not read the process state: " + ex.GetType().Name + ".";
            }
        }

        private static string TrySave(Func<string> save, string what)
        {
            try
            {
                return "Saved " + what + ": " + save();
            }
            catch (Exception ex)
            {
                return "Could not save " + what + ": " + ex.Message;
            }
        }

        // Close() is refused while a modal is open, so fall back to Kill() rather than leak a process.
        private void ShutDownApplication()
        {
            try
            {
                Application.Close();

                if (Retry.WhileFalse(() => Application.HasExited, TimeSpan.FromSeconds(5)).Success)
                {
                    return;
                }
            }
            catch (Exception)
            {
                // Fall through to Kill.
            }

            Kill(Application);
        }

        private static void Kill(Application application)
        {
            try
            {
                application.Kill();
            }
            catch (Exception)
            {
                // Already gone.
            }
        }
    }
}
