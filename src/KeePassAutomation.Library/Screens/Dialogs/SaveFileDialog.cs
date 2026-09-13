using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Windows' save dialog, which KeePass opens when creating a new database.
    public sealed class SaveFileDialog : ScreenObject
    {
        private readonly Element _fileName;

        public SaveFileDialog(AppSession session) : base(session, "Create New Database")
        {
            _fileName = ById("1001");
        }

        public void TypeFileName(string path)
        {
            _fileName.Enter(path);
        }
    }
}
