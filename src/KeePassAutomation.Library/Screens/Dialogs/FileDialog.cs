using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Windows' own open/save dialog. Its file name box has Windows' fixed ID: 1001 (save) or 1148 (open).
    public sealed class FileDialog : ScreenObject
    {
        public FileDialog(Window window) : base(window)
        {
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
    }
}
