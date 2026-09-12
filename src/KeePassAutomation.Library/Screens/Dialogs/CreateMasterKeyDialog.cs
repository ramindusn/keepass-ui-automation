using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The master key step of creating a new database (KeyCreationForm).
    public sealed class CreateMasterKeyDialog : ScreenObject
    {
        public const string Title = "Create Master Key";

        public CreateMasterKeyDialog(Window window) : base(window)
        {
        }

        public void SetPassword(string password)
        {
            TypeInto("m_tbPassword", password);
            TypeInto("m_tbRepeatPassword", password);
            Find("m_btnOK").Click();
        }
    }
}
