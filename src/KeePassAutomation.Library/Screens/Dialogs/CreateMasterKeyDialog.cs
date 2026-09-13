using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The master key step of creating a new database (KeyCreationForm).
    public sealed class CreateMasterKeyDialog : ScreenObject
    {
        private readonly Element _password;
        private readonly Element _repeatPassword;
        private readonly Element _ok;

        public CreateMasterKeyDialog(AppSession session) : base(session, "Create Master Key")
        {
            _password = ById("m_tbPassword");
            _repeatPassword = ById("m_tbRepeatPassword");
            _ok = ById("m_btnOK");
        }

        public void TypePassword(string password)
        {
            _password.Type(password);
        }

        public void TypeRepeatPassword(string password)
        {
            _repeatPassword.Type(password);
        }

        public void ClickOk()
        {
            _ok.Click();
        }
    }
}
