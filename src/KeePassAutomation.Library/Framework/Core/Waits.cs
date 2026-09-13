using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;

namespace KeePassAutomation.Framework.Core
{
    // Every wait goes through here; there is no Thread.Sleep anywhere.
    public static class Waits
    {
        // How long to keep looking before giving up. BaseTest sets it from TestConfig before each test.
        public static TimeSpan Default { get; set; } = TimeSpan.FromSeconds(10);

        // Polls until get returns something, and returns it. On timeout the error shows the UIA tree under root.
        public static T For<T>(AutomationElement root, Func<T> get, string what) where T : class
        {
            var found = Retry.WhileNull(get, Default);

            if (found.Result == null)
            {
                throw new ElementNotFoundException(what, root, Default);
            }

            return found.Result;
        }

        // Polls until the condition holds. On timeout the error shows the UIA tree under root.
        public static void Until(AutomationElement root, Func<bool> condition, string what)
        {
            if (!Retry.WhileFalse(condition, Default).Success)
            {
                throw new ElementNotFoundException(what, root, Default);
            }
        }
    }
}
