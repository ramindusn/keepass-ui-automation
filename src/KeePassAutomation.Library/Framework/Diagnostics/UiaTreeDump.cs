using System;
using System.IO;
using System.Text;
using FlaUI.Core.AutomationElements;

namespace KeePassAutomation.Framework.Diagnostics
{
    // Renders a UIA subtree as text, to diagnose runs nobody could watch.
    public static class UiaTreeDump
    {
        public static string Describe(AutomationElement root, int maxDepth = 4)
        {
            var builder = new StringBuilder();
            Walk(root, 0, maxDepth, builder);
            return builder.ToString();
        }

        public static string WriteTo(string directory, string fileName, AutomationElement root, int maxDepth = 8)
        {
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, fileName);
            File.WriteAllText(path, Describe(root, maxDepth));
            return path;
        }

        // One element, without its children.
        public static string DescribeElement(AutomationElement element)
        {
            return Line(element);
        }

        private static void Walk(AutomationElement element, int depth, int maxDepth, StringBuilder builder)
        {
            builder.Append(' ', depth * 2).AppendLine(Line(element));

            if (depth >= maxDepth)
            {
                return;
            }

            foreach (var child in Children(element))
            {
                Walk(child, depth + 1, maxDepth, builder);
            }
        }

        private static AutomationElement[] Children(AutomationElement element)
        {
            // A window can close mid-walk; a stale element throws.
            try
            {
                return element.FindAllChildren();
            }
            catch (Exception)
            {
                return Array.Empty<AutomationElement>();
            }
        }

        private static string Line(AutomationElement element)
        {
            // Property by property: menu items, toolbar buttons and list rows are not windows, so reading
            // one property they don't support would otherwise blank the whole line, name included.
            try
            {
                var properties = element.Properties;

                return properties.ControlType.ValueOrDefault
                    + " id='" + properties.AutomationId.ValueOrDefault + "'"
                    + " name='" + properties.Name.ValueOrDefault + "'"
                    + " class='" + properties.ClassName.ValueOrDefault + "'";
            }
            catch (Exception ex)
            {
                return "<unreadable element: " + ex.GetType().Name + ">";
            }
        }
    }
}
