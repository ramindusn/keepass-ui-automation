using System.IO;
using FlaUI.Core.Capturing;

namespace KeePassAutomation.Framework.Diagnostics
{
    public static class Screenshots
    {
        public static string CaptureScreen(string directory, string name)
        {
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, FileNames.Sanitise(name) + ".png");

            Capture.Screen().ToFile(path);

            return path;
        }
    }
}
