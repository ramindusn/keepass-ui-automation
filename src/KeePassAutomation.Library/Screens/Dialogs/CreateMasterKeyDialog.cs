using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The master key step of creating a new database (KeyCreationForm).
    public sealed class CreateMasterKeyDialog : ScreenObject
    {
        public const string Title = "Create Master Key";

        private readonly MainWindow _owner;

        public CreateMasterKeyDialog(MainWindow owner)
        {
            _owner = owner;
        }

        public void TypePassword(string password)
        {
            TypeInto("m_tbPassword", password);
        }

        public void TypeRepeatPassword(string password)
        {
            TypeInto("m_tbRepeatPassword", password);
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
