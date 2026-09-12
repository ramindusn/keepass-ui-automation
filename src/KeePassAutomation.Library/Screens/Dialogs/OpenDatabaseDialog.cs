using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // The master key prompt (KeyPromptForm), titled "Open Database - <file name>".
    public sealed class OpenDatabaseDialog : ScreenObject
    {
        public const string Title = "Open Database";

        private readonly AppSession _session;

        public OpenDatabaseDialog(AppSession session)
        {
            _session = session;
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
            return _session.WaitForWindow(Title);
        }
    }
}
