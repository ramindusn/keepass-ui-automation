using System;
using System.Collections.Generic;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens
{
    // One action per method. The sequence of actions that makes a scenario is written in the test.
    public sealed class MainWindow : ScreenObject
    {
        private const string GroupTree = "m_tvGroups";
        private const string EntryList = "m_lvEntries";
        private const string MainMenu = "m_menuMain";
        private const string Toolbar = "m_toolMain";

        public MainWindow(Window window)
        {
            Window = window;
        }

        public Window Window { get; }

        public string Title
        {
            get { return Window.Title; }
        }

        // Judged by the group tree, not the title, which KeePass shortens with "..." when long.
        public bool IsDatabaseOpen
        {
            get { return Find(GroupTree).FindFirstChild(cf => cf.ByControlType(ControlType.TreeItem)) != null; }
        }

        // Each row's name is its Title column.
        public IReadOnlyList<string> EntryTitles
        {
            get
            {
                var titles = new List<string>();

                foreach (var row in FindRows(EntryList))
                {
                    titles.Add(row.Name);
                }

                return titles;
            }
        }

        // Menu and toolbar items have no AutomationId, so they're found by name inside their bar.
        // Clicked, not invoked: invoking something that opens a modal dialog can block until it closes.
        public void ClickToolbarButton(string name)
        {
            FindByName(Find(Toolbar), ControlType.Button, name).Click();
        }

        public void ClickMenuItem(string menu, string item)
        {
            var parent = FindByName(Find(MainMenu), ControlType.MenuItem, menu).AsMenuItem();
            parent.Expand();

            var opened = Waits.For(Root, () => FindOpenedMenuItem(parent, item), "MenuItem named '" + item + "'");

            opened.Click();
        }

        // Groups have no AutomationId, so they're found by name inside the tree.
        public void SelectGroup(string groupName)
        {
            var tree = Find(GroupTree);
            var group = FindByName(tree, ControlType.TreeItem, groupName).AsTreeItem();

            group.Click();
            Waits.Until(tree, () => group.IsSelected, "group '" + groupName + "' to be selected");
        }

        public void SelectEntry(string title)
        {
            FindByName(Find(EntryList), ControlType.ListItem, title).Click();
        }

        public void PressEnter()
        {
            Keyboard.Type(VirtualKeyShort.RETURN);
        }

        // On failure the error lists what the entry list actually holds.
        public void WaitForEntryRow(string title)
        {
            FindByName(Find(EntryList), ControlType.ListItem, title);
        }

        public void WaitForDatabaseToOpen()
        {
            Waits.Until(Root, () => IsDatabaseOpen, "a database to open");
        }

        public void WaitForDatabaseToClose()
        {
            Waits.Until(Root, () => !IsDatabaseOpen, "the database to close");
        }

        // Unsaved changes show as "*" before " - KeePass" in the title.
        public void WaitForDatabaseToBeSaved()
        {
            Waits.Until(Root, () => !Title.EndsWith("* - KeePass", StringComparison.Ordinal), "the database to be saved");
        }

        protected override AutomationElement Locate()
        {
            return Window;
        }

        // An open dropdown is a popup, which WinForms parents under the menu item or under the window.
        private AutomationElement FindOpenedMenuItem(AutomationElement parent, string item)
        {
            var underParent = FindMenuItem(parent, item);

            if (underParent != null)
            {
                return underParent;
            }

            return FindMenuItem(Root, item);
        }

        private static AutomationElement FindMenuItem(AutomationElement container, string name)
        {
            return container.FindFirstDescendant(cf => cf.ByControlType(ControlType.MenuItem).And(cf.ByName(name)));
        }
    }
}
