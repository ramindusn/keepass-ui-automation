using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;

namespace KeePassAutomation.Framework.Core
{
    // A control described by how to find it, and found again on every action. Nothing is looked up
    // when it is declared, so a page can list its controls before its window exists. Every lookup
    // waits, so no action needs its own wait.
    public sealed class Element
    {
        private readonly Func<AutomationElement> _find;

        public Element(Func<AutomationElement> find)
        {
            _find = find;
        }

        public string Text
        {
            get { return _find().AsTextBox().Text; }
        }

        public void Click()
        {
            _find().Click();
        }

        // Set through UI Automation, for ordinary text boxes.
        public void SetText(string text)
        {
            _find().AsTextBox().Text = text;
        }

        // Typed with the keyboard, for boxes that keep their own buffer and ignore text set via UIA.
        public void Type(string text)
        {
            _find().Focus();
            Keyboard.Type(text);
            Wait.UntilInputIsProcessed();
        }

        // Typed into a cleared box, for the file name box of Windows' file dialog.
        public void Enter(string text)
        {
            var element = _find();

            element.Focus();
            element.AsTextBox().Enter(text);
        }

        public void Select()
        {
            _find().AsTabItem().Select();
        }
    }
}
