using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace KeePassAutomation.Framework.Core
{
    // A page object. It locates its own window when an action is called, so a test can hold one
    // object per window or dialog for its whole run, and every search is scoped to that window.
    // A page declares its controls as Elements in its constructor and acts on them in its methods.
    public abstract class ScreenObject
    {
        protected AutomationElement Root
        {
            get { return Locate(); }
        }

        // The window this object stands for; for a dialog, found by title and waited for.
        protected abstract AutomationElement Locate();

        protected Element ById(string automationId)
        {
            return new Element(() => Find(automationId));
        }

        // For controls without an AutomationId (toolbar buttons, menu items, tree and list items),
        // found by type and name only inside their container.
        protected Element ByName(string containerId, ControlType type, string name)
        {
            return new Element(() => FindByName(Find(containerId), type, name));
        }

        protected AutomationElement Find(string automationId, TimeSpan? timeout = null)
        {
            return Waits.ForDescendant(Root, automationId, timeout);
        }

        protected static AutomationElement FindByName(AutomationElement container, ControlType type, string name, TimeSpan? timeout = null)
        {
            return Waits.For(container,
                () => container.FindFirstDescendant(cf => cf.ByControlType(type).And(cf.ByName(name))),
                type + " named '" + name + "'",
                timeout);
        }

        // The rows of a list view, in display order.
        protected AutomationElement[] FindRows(string listAutomationId)
        {
            return Find(listAutomationId).FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));
        }
    }
}
