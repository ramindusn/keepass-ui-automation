using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using KeePassAutomation.Framework.AppUnderTest;

namespace KeePassAutomation.Framework.Core
{
    // The base of every screen, the MainWindow screen and each dialog alike. A screen declares its
    // controls with ById or ByName and uses them; finding windows and waiting happen here.
    public abstract class ScreenObject
    {
        private readonly AppSession _session;
        private readonly string[] _titles;

        // Dialogs pass their window title. A screen with no title uses the window KeePass opened at launch.
        protected ScreenObject(AppSession session, params string[] titles)
        {
            _session = session;
            _titles = titles;
        }

        // The title of this screen's window.
        protected string WindowTitle
        {
            get { return Root.Name; }
        }

        // A control, looked up by its automation id when it's used.
        protected Element ById(string automationId)
        {
            return new Element(() => Find(automationId));
        }

        // A control without an automation id, looked up by its type and name.
        protected Element ByName(string containerId, ControlType type, string name)
        {
            return ById(containerId).Child(type, name);
        }

        // A control anywhere in this screen's window, looked up by its type and name.
        protected Element ByName(ControlType type, string name)
        {
            return new Element(() => Root).Child(type, name);
        }

        // Waits until the condition holds. On timeout the error shows this screen's window.
        protected void WaitUntil(Func<bool> condition, string what)
        {
            Waits.Until(Root, condition, what);
        }

        // Presses Enter on the focused control.
        public void PressEnter()
        {
            Keyboard.Type(VirtualKeyShort.RETURN);
        }

        // This screen's window, looked up again on each action.
        private AutomationElement Root
        {
            get
            {
                // No title: this is the MainWindow screen, so use the window KeePass opened at launch.
                if (_titles.Length == 0)
                {
                    return _session.AppWindow;
                }

                // A title: this is a dialog, so find its window by that title.
                return _session.WaitForWindow(_titles);
            }
        }

        // Waits for a control with this automation id and returns it.
        private AutomationElement Find(string automationId)
        {
            var root = Root;

            return Waits.For(root,
                () => root.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
                "element with AutomationId '" + automationId + "'");
        }
    }
}
