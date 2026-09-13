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
            // Windows' own id for the file name box in an open dialog.
            _fileName = ById("1148");
        }

        public void TypeFileName(string path)
        {
            _fileName.Enter(path);
        }
    }
}
