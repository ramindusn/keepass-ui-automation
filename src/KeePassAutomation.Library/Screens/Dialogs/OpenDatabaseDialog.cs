using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The master key prompt (KeyPromptForm), titled "Open Database - <file name>".
    public sealed class OpenDatabaseDialog : ScreenObject
    {
        private readonly Element _password;
        private readonly Element _ok;

        public OpenDatabaseDialog(AppSession session) : base(session, "Open Database")
        {
            _password = ById("m_tbPassword");
            _ok = ById("m_btnOK");
        }

        public void TypePassword(string masterPassword)
        {
            _password.Type(masterPassword);
        }

        public void ClickOk()
        {
            _ok.Click();
        }
    }
}
