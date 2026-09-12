using System;
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
        // A window of the app by the start of its title; a form that opens under several titles
        // ("Add Entry", "Edit Entry") matches any of them. An owned dialog can sit under its owner in
        // the UIA tree, on the desktop, or under another dialog, so all three places are searched.
        public static Window ForWindow(Window owner, string[] titleStarts, TimeSpan? timeout = null)
        {
            var processId = owner.Properties.ProcessId.Value;

            return For(owner, () => FindWindow(owner, processId, titleStarts),
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

        private static Window FindWindow(Window owner, int processId, string[] titleStarts)
        {
            foreach (var modal in owner.ModalWindows)
            {
                if (HasTitleStarting(modal, titleStarts))
                {
                    return modal;
                }
            }

            var onDesktop = owner.Automation.GetDesktop()
                .FindAllChildren(cf => cf.ByControlType(ControlType.Window).And(cf.ByProcessId(processId)));

            foreach (var window in onDesktop)
            {
                if (!window.Equals(owner) && HasTitleStarting(window, titleStarts))
                {
                    return window.AsWindow();
                }
            }

            var nested = owner.FindAllDescendants(cf => cf.ByControlType(ControlType.Window));

            foreach (var window in nested)
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
