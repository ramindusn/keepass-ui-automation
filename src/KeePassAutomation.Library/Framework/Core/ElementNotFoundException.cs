using System;
using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Diagnostics;

namespace KeePassAutomation.Framework.Core
{
    // Includes the surrounding UIA tree, so a locator failure read from a CI log is actionable.
    public sealed class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string what, AutomationElement searchRoot, TimeSpan timeout)
            : base(Build(what, searchRoot, timeout))
        {
        }

        private static string Build(string what, AutomationElement searchRoot, TimeSpan timeout)
        {
            var root = Safe(() => UiaTreeDump.DescribeElement(searchRoot));
            var tree = Safe(() => UiaTreeDump.Describe(searchRoot));

            return "Could not find " + what + " within " + timeout.TotalSeconds.ToString("0.#") + "s."
                + Environment.NewLine + Environment.NewLine
                + "Searched under: " + root
                + Environment.NewLine + Environment.NewLine
                + "What was actually there:" + Environment.NewLine
                + tree;
        }

        private static string Safe(Func<string> describe)
        {
            try
            {
                return describe();
            }
            catch (Exception ex)
            {
                return "<could not read: " + ex.GetType().Name + ">";
            }
        }
    }
}
