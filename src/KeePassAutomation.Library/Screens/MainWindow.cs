using System;
using System.Collections.Generic;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using KeePassAutomation.Framework.Core;
using KeePassAutomation.Screens.Dialogs;

namespace KeePassAutomation.Screens
{
    public sealed class MainWindow : ScreenObject
    {
        private const string GroupTree = "m_tvGroups";
        private const string EntryList = "m_lvEntries";
        private const string MainMenu = "m_menuMain";
        private const string Toolbar = "m_toolMain";

        // Titles of the standard file dialogs KeePass opens, and of its new-database wizard step.
        private const string CreateDatabaseFileTitle = "Create New Database";
        private const string OpenDatabaseFileTitle = "Open Database File";
        private const string ConfigureNewDatabaseTitle = "Configure New Database";

        public MainWindow(Window window) : base(window)
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

        // Groups have no AutomationId, so they're found by name inside the tree.
        public void SelectGroup(string groupName)
        {
            var tree = Find(GroupTree);
            var group = FindByName(tree, ControlType.TreeItem, groupName).AsTreeItem();

            group.Click();
            Waits.Until(tree, () => group.IsSelected, "group '" + groupName + "' to be selected");
        }

        // On failure the error lists what the entry list actually holds.
        public void WaitForEntryRow(string title)
        {
            FindByName(Find(EntryList), ControlType.ListItem, title);
        }

        // The toolbar, not the Entry menu: KeePass rebuilds that menu as it opens, and its items keep
        // reporting the names they had before, so "Add Entry..." is not findable by name.
        public EntryDialog AddEntry()
        {
            ClickToolbarButton("Add Entry");
            return new EntryDialog(Waits.ForModalWindow(Window, EntryDialog.AddTitle));
        }

        // Selected and opened with Enter: a double-click lands mid-row, where the column under the
        // pointer decides what happens.
        public EntryDialog OpenEntry(string title)
        {
            FindByName(Find(EntryList), ControlType.ListItem, title).Click();
            Keyboard.Type(VirtualKeyShort.RETURN);

            return new EntryDialog(Waits.ForModalWindow(Window, EntryDialog.EditTitle));
        }

        public FindDialog OpenFind()
        {
            ClickMenuItem("Find", "Find...");
            return new FindDialog(Waits.ForModalWindow(Window, FindDialog.Title));
        }

        // KeePass creates the database in memory only; call SaveDatabase to write it to disk.
        public void CreateDatabase(string path, string masterPassword)
        {
            ClickToolbarButton("New Database");
            new FileDialog(Waits.ForModalWindow(Window, CreateDatabaseFileTitle)).Choose(path);
            new CreateMasterKeyDialog(Waits.ForModalWindow(Window, CreateMasterKeyDialog.Title)).SetPassword(masterPassword);
            Waits.ForDescendant(Waits.ForModalWindow(Window, ConfigureNewDatabaseTitle), "m_btnOK").Click();
            WaitForDatabaseToOpen();
        }

        // Unsaved changes show as "*" before " - KeePass" in the title.
        public void SaveDatabase()
        {
            ClickToolbarButton("Save Database");
            Waits.Until(Root, () => !Title.EndsWith("* - KeePass", StringComparison.Ordinal), "the database to be saved");
        }

        public void CloseDatabase()
        {
            ClickMenuItem("File", "Close");
            Waits.Until(Root, () => !IsDatabaseOpen, "the database to close");
        }

        public OpenDatabaseDialog OpenDatabase(string path)
        {
            ClickToolbarButton("Open Database");
            new FileDialog(Waits.ForModalWindow(Window, OpenDatabaseFileTitle)).Choose(path);
            return WaitForOpenDatabasePrompt();
        }

        public OpenDatabaseDialog WaitForOpenDatabasePrompt()
        {
            return new OpenDatabaseDialog(Waits.ForModalWindow(Window, OpenDatabaseDialog.Title));
        }

        public void WaitForDatabaseToOpen()
        {
            Waits.Until(Root, () => IsDatabaseOpen, "a database to open");
        }

        // Menu and toolbar items have no AutomationId, so they're found by name inside their bar.
        // Clicked, not invoked: invoking something that opens a modal dialog can block until it closes.
        private void ClickMenuItem(string menu, string item)
        {
            var parent = FindByName(Find(MainMenu), ControlType.MenuItem, menu).AsMenuItem();
            parent.Expand();

            var opened = Waits.For(Root, () => FindOpenedMenuItem(parent, item), "MenuItem named '" + item + "'");

            opened.Click();
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

        private void ClickToolbarButton(string name)
        {
            FindByName(Find(Toolbar), ControlType.Button, name).Click();
        }
    }
}
