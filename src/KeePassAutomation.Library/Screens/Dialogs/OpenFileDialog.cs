using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Windows' open dialog, which KeePass opens when opening an existing database.
    public sealed class OpenFileDialog : ScreenObject
    {
        private readonly Element _fileName;

        public OpenFileDialog(AppSession session) : base(session, "Open Database File")
        {
            _fileName = new Element(FindFileNameBox);
        }

        public void TypeFileName(string path)
        {
            _fileName.Enter(path);
        }

        // Id 1148 is on both the file name combo box and the edit box inside it; typing needs the edit box.
        private AutomationElement FindFileNameBox()
        {
            var root = Root;

            return Waits.For(root,
                () => root.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit).And(cf.ByAutomationId("1148"))),
                "the file name box");
        }
    }
}
