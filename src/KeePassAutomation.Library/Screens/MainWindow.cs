using System;
using System.Collections.Generic;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens
{
    // KeePass's main window: one method per action.
    public sealed class MainWindow : ScreenObject
    {
        private const string GroupTree = "m_tvGroups";
        private const string EntryList = "m_lvEntries";
        private const string MainMenu = "m_menuMain";
        private const string Toolbar = "m_toolMain";

        public MainWindow(AppSession session) : base(session)
        {
        }

        public string Title
        {
            get { return Root.Name; }
        }

        // True when the group tree has groups; the title can't be trusted, as KeePass shortens it.
        public bool IsDatabaseOpen
        {
            get { return Find(GroupTree).FindFirstChild(cf => cf.ByControlType(ControlType.TreeItem)) != null; }
        }

        // The titles of the entries in the list.
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

        // Clicks a toolbar button by its name.
        public void ClickToolbarButton(string name)
        {
            ByName(Toolbar, ControlType.Button, name).Click();
        }

        // Opens a menu and clicks one of its items.
        public void ClickMenuItem(string menu, string item)
        {
            FindByName(Find(MainMenu), ControlType.MenuItem, menu).AsMenuItem().Expand();
            FindByName(Root, ControlType.MenuItem, item).Click();
        }

        // Selects a group in the tree and waits until it is selected.
        public void SelectGroup(string groupName)
        {
            var tree = Find(GroupTree);
            var group = FindByName(tree, ControlType.TreeItem, groupName).AsTreeItem();

            group.Click();
            Waits.Until(tree, () => group.IsSelected, "group '" + groupName + "' to be selected");
        }

        public void SelectEntry(string title)
        {
            ByName(EntryList, ControlType.ListItem, title).Click();
        }

        // Waits until an entry with this title appears in the list.
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

        // Waits until the title loses its "*", which marks unsaved changes.
        public void WaitForDatabaseToBeSaved()
        {
            Waits.Until(Root, () => !Title.EndsWith("* - KeePass", StringComparison.Ordinal), "the database to be saved");
        }
    }
}
