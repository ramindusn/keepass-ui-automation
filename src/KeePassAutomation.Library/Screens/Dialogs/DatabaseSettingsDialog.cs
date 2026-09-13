using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The settings step of creating a new database (DatabaseSettingsForm); the defaults are accepted.
    public sealed class DatabaseSettingsDialog : ScreenObject
    {
        private readonly Element _ok;

        public DatabaseSettingsDialog(AppSession session) : base(session, "Configure New Database")
        {
            _ok = ById("m_btnOK");
        }

        public void ClickOk()
        {
            _ok.Click();
        }
    }
}
