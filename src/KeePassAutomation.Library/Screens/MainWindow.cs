using FlaUI.Core.AutomationElements;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Screens
{
    public sealed class MainWindow : ScreenObject
    {
        public MainWindow(Window window) : base(window)
        {
            Window = window;
        }

        public Window Window { get; }

        public string Title
        {
            get { return Window.Title; }
        }
    }
}
