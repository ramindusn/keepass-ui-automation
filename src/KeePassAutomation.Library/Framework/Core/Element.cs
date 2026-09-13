using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;

namespace KeePassAutomation.Framework.Core
{
    // A button, text box or tab on a screen, which a test can click, type into or read.
    public sealed class Element
    {
        private readonly Func<AutomationElement> _find;

        // Takes how to find the control.
        public Element(Func<AutomationElement> find)
        {
            _find = find;
        }

        // Reads the control's text.
        public string Text
        {
            get { return _find().AsTextBox().Text; }
        }

        // Clicks the control.
        public void Click()
        {
            _find().Click();
        }

        // Sets the text directly, for ordinary text boxes.
        public void SetText(string text)
        {
            _find().AsTextBox().Text = text;
        }

        // Types with the keyboard, for password boxes that ignore text set directly.
        public void Type(string text)
        {
            _find().Focus();
            Keyboard.Type(text);
            Wait.UntilInputIsProcessed();
        }

        // Clears the box and types into it, for the file name box in Windows' file dialogs.
        public void Enter(string text)
        {
            var element = _find();

            element.Focus();
            element.AsTextBox().Enter(text);
        }

        // Selects the control, such as a tab.
        public void Select()
        {
            _find().AsTabItem().Select();
        }
    }
}
