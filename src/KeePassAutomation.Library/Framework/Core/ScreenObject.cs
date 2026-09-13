using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using KeePassAutomation.Framework.AppUnderTest;

namespace KeePassAutomation.Framework.Core
{
    // The base of every screen, the main window and each dialog alike. It finds its window when an
    // action runs, and searches only inside it.
    public abstract class ScreenObject
    {
        private readonly AppSession _session;
        private readonly string[] _titles;

        // Dialogs pass their window title; the main window passes none.
        protected ScreenObject(AppSession session, params string[] titles)
        {
            _session = session;
            _titles = titles;
        }

        // This screen's window, looked up again on each action.
        protected AutomationElement Root
        {
            get
            {
                if (_titles.Length == 0)
                {
                    return _session.MainWindow;
                }

                return _session.WaitForWindow(_titles);
            }
        }

        // A control, looked up by its automation id when it's used.
        protected Element ById(string automationId)
        {
            return new Element(() => Find(automationId));
        }

        // A control without an automation id, looked up by its type and name.
        protected Element ByName(string containerId, ControlType type, string name)
        {
            return new Element(() => FindByName(Find(containerId), type, name));
        }

        // Waits for a control with this automation id and returns it.
        protected AutomationElement Find(string automationId)
        {
            var root = Root;

            return Waits.For(root,
                () => root.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
                "element with AutomationId '" + automationId + "'");
        }

        // Waits for a control with this type and name and returns it.
        protected static AutomationElement FindByName(AutomationElement container, ControlType type, string name)
        {
            return Waits.For(container,
                () => container.FindFirstDescendant(cf => cf.ByControlType(type).And(cf.ByName(name))),
                type + " named '" + name + "'");
        }

        // Presses Enter on the focused control.
        public void PressEnter()
        {
            Keyboard.Type(VirtualKeyShort.RETURN);
        }

        // The rows of a list, top to bottom.
        protected AutomationElement[] FindRows(string listAutomationId)
        {
            return Find(listAutomationId).FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));
        }
    }
}
