using System;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Framework.AppUnderTest
{
    // One running KeePass per test: started, brought to the front, searched for dialogs, then killed.
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

        // If launch fails, KeePass is killed here, because the caller never gets a session to dispose.
        public static AppSession Launch(TimeSpan launchTimeout)
        {
            var application = Application.Launch(KeePassPackage.FindExecutable());
            var automation = new UIA3Automation();

            try
            {
                // Without a timeout FlaUI waits forever on an unexpected dialog.
                var mainWindow = application.GetMainWindow(automation, launchTimeout);

                if (mainWindow == null)
                {
                    throw new InvalidOperationException(
                        "KeePass started but showed no main window within " + launchTimeout.TotalSeconds + "s.");
                }

                // Clicks land on whichever window is in front, so KeePass has to be there.
                Waits.Until(mainWindow, () => TryBringToFront(mainWindow, automation, application.ProcessId), "KeePass to come to the front");

                return new AppSession(application, automation, mainWindow);
            }
            catch (Exception)
            {
                automation.Dispose();
                Kill(application);
                application.Dispose();
                throw;
            }
        }

        // A window of this app whose title starts with any of the given titles.
        public Window WaitForWindow(params string[] titleStarts)
        {
            return Waits.For(MainWindow,
                () => FindWindow(titleStarts),
                "a window titled '" + string.Join("…' or '", titleStarts) + "…'");
        }

        // Killed, not closed: a close is refused while a dialog is open.
        public void Dispose()
        {
            Kill(_application);
            _automation.Dispose();
            _application.Dispose();
        }

        // A dialog can sit under the main window, on the desktop, or under another dialog.
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

        // A window still being created has no name yet; skip it and let the poll try again.
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

        // Windows can refuse focus without saying so; restoring a minimised window is always allowed.
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
