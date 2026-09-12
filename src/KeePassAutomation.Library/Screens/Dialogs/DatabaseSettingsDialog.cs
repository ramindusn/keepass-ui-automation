using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The settings step of creating a new database (DatabaseSettingsForm); the defaults are accepted.
    public sealed class DatabaseSettingsDialog : ScreenObject
    {
        public const string Title = "Configure New Database";

        private readonly AppSession _session;
        private readonly Element _ok;

        public DatabaseSettingsDialog(AppSession session)
        {
            _session = session;
            _ok = ById("m_btnOK");
        }

        public void ClickOk()
        {
            _ok.Click();
        }

        protected override AutomationElement Locate()
        {
            return _session.WaitForWindow(Title);
        }
    }
}
