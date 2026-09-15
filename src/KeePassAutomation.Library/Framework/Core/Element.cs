using System;
using System.Collections.Generic;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;

namespace KeePassAutomation.Framework.Core
{
    // A control on a screen, such as a button, text box, tab, menu, tree or list.
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

        // True when the control, such as a tree item, is selected.
        public bool IsSelected
        {
            get { return _find().AsTreeItem().IsSelected; }
        }

        // A control inside this one, found by its type and name when it's used.
        public Element Child(ControlType type, string name)
        {
            return new Element(() =>
            {
                var container = _find();

                return Waits.For(container,
                    () => container.FindFirstDescendant(cf => cf.ByControlType(type).And(cf.ByName(name))),
                    type + " named '" + name + "'");
            });
        }

        // The row at this position in a list, counting from 0, found when it's used.
        public Element Row(int index)
        {
            return new Element(() =>
            {
                var list = _find();

                return Waits.For(list, () => RowOrNull(list, index), "row " + (index + 1) + " of the list");
            });
        }

        // True when the control has at least one direct child of this type.
        public bool HasChild(ControlType type)
        {
            return _find().FindFirstChild(cf => cf.ByControlType(type)) != null;
        }

        // The names of the controls of this type inside this one, top to bottom.
        public IReadOnlyList<string> ChildNames(ControlType type)
        {
            var names = new List<string>();

            foreach (var child in _find().FindAllDescendants(cf => cf.ByControlType(type)))
            {
                names.Add(child.Name);
            }

            return names;
        }

        // Waits until the control appears.
        public void WaitToAppear()
        {
            _find();
        }

        // Clicks the control.
        public void Click()
        {
            _find().Click();
        }

        // Opens the control, such as a menu.
        public void Expand()
        {
            _find().AsMenuItem().Expand();
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

        // Null until the list has a row at this position.
        private static AutomationElement RowOrNull(AutomationElement list, int index)
        {
            var rows = list.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));

            if (index >= rows.Length)
            {
                return null;
            }

            return rows[index];
        }
    }
}
