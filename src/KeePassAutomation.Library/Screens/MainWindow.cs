using System;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using KeePassAutomation.Framework.Core;
using KeePassAutomation.Screens.Dialogs;

namespace KeePassAutomation.Screens
{
    public sealed class MainWindow : ScreenObject
    {
        private const string GroupTree = "m_tvGroups";
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
