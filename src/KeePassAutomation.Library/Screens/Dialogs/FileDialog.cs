using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Windows' own open/save dialog, under the two titles KeePass gives it.
    // Its file name box has Windows' fixed ID: 1001 (save) or 1148 (open).
    public sealed class FileDialog : ScreenObject
    {
        private static readonly string[] Titles = { "Create New Database", "Open Database File" };

        private readonly MainWindow _owner;

        public FileDialog(MainWindow owner)
        {
            _owner = owner;
        }

        public void TypeFileName(string path)
        {
            var fileName = Waits.For(Root,
                () => Root.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit)
                    .And(cf.ByAutomationId("1001").Or(cf.ByAutomationId("1148")))),
                "the file name box");

            fileName.Focus();
            fileName.AsTextBox().Enter(path);
        }

        public void PressEnter()
        {
            Keyboard.Type(VirtualKeyShort.ENTER);
        }

        protected override AutomationElement Locate()
        {
            return Waits.ForModalWindow(_owner.Window, Titles);
        }
    }
}
