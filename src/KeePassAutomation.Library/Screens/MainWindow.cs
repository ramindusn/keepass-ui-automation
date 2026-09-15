using System;
using System.Collections.Generic;
using FlaUI.Core.Definitions;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens
{
    // KeePass's main window: one method per action.
    public sealed class MainWindow : ScreenObject
    {
        private readonly Element _groupTree;
        private readonly Element _entryList;
        private readonly Element _mainMenu;
        private readonly Element _toolbar;

        public MainWindow(AppSession session) : base(session)
        {
            _groupTree = ById("m_tvGroups");
            _entryList = ById("m_lvEntries");
            _mainMenu = ById("m_menuMain");
            _toolbar = ById("m_toolMain");
        }

        public string Title
        {
            get { return WindowTitle; }
        }

        // True when the group tree has groups; the title can't be trusted, as KeePass shortens it.
        public bool IsDatabaseOpen
        {
            get { return _groupTree.HasChild(ControlType.TreeItem); }
        }

        // The titles of the entries in the list.
        public IReadOnlyList<string> EntryTitles
        {
            get { return _entryList.ChildNames(ControlType.ListItem); }
        }

        // Clicks a toolbar button by its name.
        public void ClickToolbarButton(string name)
        {
            _toolbar.Child(ControlType.Button, name).Click();
        }

        // Opens a menu and clicks one of its items.
        public void ClickMenuItem(string menu, string item)
        {
            _mainMenu.Child(ControlType.MenuItem, menu).Expand();

            // An open menu's items sit under the window, not under the menu bar.
            ByName(ControlType.MenuItem, item).Click();
        }

        // Selects a group in the tree and waits until it is selected.
        public void SelectGroup(string groupName)
        {
            var group = _groupTree.Child(ControlType.TreeItem, groupName);

            group.Click();
            WaitUntil(() => group.IsSelected, "group '" + groupName + "' to be selected");
        }

        // Selects an entry in the list by its title.
        public void SelectEntry(string title)
        {
            _entryList.Child(ControlType.ListItem, title).Click();
        }

        // Waits until an entry with this title appears in the list.
        public void WaitForEntryRow(string title)
        {
            _entryList.Child(ControlType.ListItem, title).WaitToAppear();
        }

        // Waits until the group tree shows an open database.
        public void WaitForDatabaseToOpen()
        {
            WaitUntil(() => IsDatabaseOpen, "a database to open");
        }

        // Waits until the group tree is empty again.
        public void WaitForDatabaseToClose()
        {
            WaitUntil(() => !IsDatabaseOpen, "the database to close");
        }

        // Waits until the title loses its "*", which marks unsaved changes.
        public void WaitForDatabaseToBeSaved()
        {
            WaitUntil(() => !Title.EndsWith("* - KeePass", StringComparison.Ordinal), "the database to be saved");
        }
    }
}
