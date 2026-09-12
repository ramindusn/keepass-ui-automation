using System;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;

namespace KeePassAutomation.Framework.Core
{
    // Every wait goes through here; there is no Thread.Sleep anywhere.
    public static class Waits
    {
        public static TimeSpan Default { get; set; } = TimeSpan.FromSeconds(10);

        public static AutomationElement ForDescendant(AutomationElement root, string automationId, TimeSpan? timeout = null)
        {
            var limit = Limit(timeout);

            var found = Retry.WhileNull(
                () => root.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
                limit);

            if (found.Result == null)
            {
                throw new ElementNotFoundException("element with AutomationId '" + automationId + "'", root, limit);
            }

            return found.Result;
        }

        public static T For<T>(AutomationElement root, Func<T> get, string what, TimeSpan? timeout = null) where T : class
        {
            var limit = Limit(timeout);
            var found = Retry.WhileNull(get, limit);

            if (found.Result == null)
            {
                throw new ElementNotFoundException(what, root, limit);
            }

            return found.Result;
        }

        // Matches the start of the title. Owned dialogs aren't always under their owner in the UIA tree,
        // so the owner's process is searched on the desktop too, excluding the owner itself.
        // A top-level window of the app under test, by the start of its title. Owned dialogs are
        // top-level windows too, so this finds every KeePass dialog. A form that opens under several
        // titles ("Add Entry", "Edit Entry") matches any of them.
        public static Window ForWindow(AutomationBase automation, int processId, string[] titleStarts, TimeSpan? timeout = null)
        {
            var desktop = automation.GetDesktop();

            return For(desktop, () => FindWindow(desktop, processId, titleStarts),
                "a window titled '" + string.Join("…' or '", titleStarts) + "…'",
                timeout);
        }

        public static void Until(AutomationElement root, Func<bool> condition, string what, TimeSpan? timeout = null)
        {
            var limit = Limit(timeout);

            if (!Retry.WhileFalse(condition, limit).Success)
            {
                throw new ElementNotFoundException(what, root, limit);
            }
        }

        private static Window FindWindow(AutomationElement desktop, int processId, string[] titleStarts)
        {
            var windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window).And(cf.ByProcessId(processId)));

            foreach (var window in windows)
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

        private static TimeSpan Limit(TimeSpan? timeout)
        {
            if (timeout == null)
            {
                return Default;
            }

            return timeout.Value;
        }
    }
}
