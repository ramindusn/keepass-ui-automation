using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The settings step of creating a new database (DatabaseSettingsForm); the defaults are accepted.
    public sealed class DatabaseSettingsDialog : ScreenObject
    {
        public const string Title = "Configure New Database";

        private readonly MainWindow _owner;

        public DatabaseSettingsDialog(MainWindow owner)
        {
            _owner = owner;
        }

        public void ClickOk()
        {
            Find("m_btnOK").Click();
        }

        protected override AutomationElement Locate()
        {
            return Waits.ForModalWindow(_owner.Window, Title);
        }
    }
}
