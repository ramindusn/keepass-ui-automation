using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;

namespace KeePassAutomation.Framework.Core
{
    // Takes an AutomationElement, not a Window, so windows, panels and dialogs share one base
    // and every search is scoped to its own subtree.
    public abstract class ScreenObject
    {
        protected ScreenObject(AutomationElement root)
        {
            Root = root;
        }

        protected AutomationElement Root { get; }

        protected AutomationElement Find(string automationId, TimeSpan? timeout = null)
        {
            return Waits.ForDescendant(Root, automationId, timeout);
        }

        // The rows of a list view, in display order.
        protected AutomationElement[] FindRows(string listAutomationId)
        {
            return Find(listAutomationId).FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));
        }

        // For elements without an AutomationId (toolbar buttons, menu items), searched only inside a container.
        protected static AutomationElement FindByName(AutomationElement container, ControlType type, string name, TimeSpan? timeout = null)
        {
            return Waits.For(container,
                () => container.FindFirstDescendant(cf => cf.ByControlType(type).And(cf.ByName(name))),
                type + " named '" + name + "'",
                timeout);
        }

        // Typed with the keyboard: KeePass's password boxes keep their own buffer and ignore text set via UIA.
        protected void TypeInto(string automationId, string text)
        {
            Find(automationId).Focus();
            Keyboard.Type(text);
            Wait.UntilInputIsProcessed();
        }
    }
}
