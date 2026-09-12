using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The master key prompt (KeyPromptForm), titled "Open Database - <file name>".
    public sealed class OpenDatabaseDialog : ScreenObject
    {
        public const string Title = "Open Database";

        public OpenDatabaseDialog(Window window) : base(window)
        {
        }

        public void Unlock(string masterPassword)
        {
            TypeInto("m_tbPassword", masterPassword);
            Find("m_btnOK").Click();
        }
    }
}
