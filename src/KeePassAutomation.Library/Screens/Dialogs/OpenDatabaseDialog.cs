using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The master key prompt (KeyPromptForm), titled "Open Database - <file name>".
    public sealed class OpenDatabaseDialog : ScreenObject
    {
        public const string Title = "Open Database";

        private readonly MainWindow _owner;

        public OpenDatabaseDialog(MainWindow owner)
        {
            _owner = owner;
        }

        public void TypePassword(string masterPassword)
        {
            TypeInto("m_tbPassword", masterPassword);
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
