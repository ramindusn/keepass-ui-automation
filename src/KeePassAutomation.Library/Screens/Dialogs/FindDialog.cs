using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens.Dialogs
{
    // Find > Find... (SearchForm). Results replace the contents of the entry list.
    public sealed class FindDialog : ScreenObject
    {
        public const string Title = "Find";

        public FindDialog(Window window) : base(window)
        {
        }

        public void SetSearchText(string text)
        {
            Find("m_tbSearch").AsTextBox().Text = text;
        }

        public void ClickOk()
        {
            Find("m_btnOK").Click();
        }
    }
}
