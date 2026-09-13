using System;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Framework.AppUnderTest
{
    // One running KeePass for the length of a test: started, brought to the front, searched for its
    // dialogs, and killed at the end.
    public sealed class AppSession : IDisposable
    {
        private readonly Application _application;
        private readonly UIA3Automation _automation;

        private AppSession(Application application, UIA3Automation automation, Window mainWindow)
        {
            _application = application;
            _automation = automation;
            MainWindow = mainWindow;
        }

        public Window MainWindow { get; }

        public static AppSession Launch(TimeSpan launchTimeout, TimeSpan foregroundTimeout)
        {
            var application = Application.Launch(KeePassPackage.FindExecutable());
            var automation = new UIA3Automation();

            // Without a timeout FlaUI waits forever, so one unexpected dialog would hang the run.
            var mainWindow = application.GetMainWindow(automation, launchTimeout);

            if (mainWindow == null)
            {
                automation.Dispose();
                Kill(application);
                application.Dispose();

                throw new InvalidOperationException(
                    "KeePass started but showed no main window within " + launchTimeout.TotalSeconds + "s.");
            }

            // Clicks are real mouse events and land on whichever window is in front, so KeePass is
            // brought there, and that is checked rather than assumed.
            Retry.WhileFalse(() => TryBringToFront(mainWindow, automation, application.ProcessId), foregroundTimeout);

            return new AppSession(application, automation, mainWindow);
        }

        // Waits for a window of this app by the start of its title; a form that opens under several
        // titles ("Add Entry", "Edit Entry") matches any of them.
        public Window WaitForWindow(params string[] titleStarts)
        {
            return Waits.For(MainWindow,
                () => FindWindow(titleStarts),
                "a window titled '" + string.Join("…' or '", titleStarts) + "…'");
        }

        // Killed, not closed: every test starts a fresh KeePass, and a close is refused while a dialog is open.
        public void Dispose()
        {
            Kill(_application);
            _automation.Dispose();
            _application.Dispose();
        }

        // An owned dialog can sit under the main window in the UIA tree, on the desktop, or under
        // another dialog, so all three places are searched.
        private Window FindWindow(string[] titleStarts)
        {
            foreach (var modal in MainWindow.ModalWindows)
            {
                if (HasTitleStarting(modal, titleStarts))
                {
                    return modal;
                }
            }

            var onDesktop = _automation.GetDesktop()
                .FindAllChildren(cf => cf.ByControlType(ControlType.Window).And(cf.ByProcessId(_application.ProcessId)));

            foreach (var window in onDesktop)
            {
                if (!window.Equals(MainWindow) && HasTitleStarting(window, titleStarts))
                {
                    return window.AsWindow();
                }
            }

            foreach (var window in MainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Window)))
            {
                if (HasTitleStarting(window, titleStarts))
                {
                    return window.AsWindow();
                }
            }

            return null;
        }

        // A window that is still being created is already on the desktop but has no name yet, and
        // reading it throws. Read without throwing and skip it, so the poll simply tries again.
        private static bool HasTitleStarting(AutomationElement window, string[] titleStarts)
        {
            var name = window.Properties.Name.ValueOrDefault;

            if (name == null)
            {
                return false;
            }

            foreach (var titleStart in titleStarts)
            {
                if (name.StartsWith(titleStart, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        // Windows refuses foreground to a process it did not just activate, and says so only by leaving
        // the window where it was. Restoring a minimised window is an activation Windows does allow.
        private static bool TryBringToFront(Window mainWindow, UIA3Automation automation, int processId)
        {
            try
            {
                mainWindow.SetForeground();

                if (HasForeground(automation, processId))
                {
                    return true;
                }

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
