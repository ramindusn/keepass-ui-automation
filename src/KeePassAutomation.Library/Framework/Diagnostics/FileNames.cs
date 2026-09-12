using System;
using System.IO;

namespace KeePassAutomation.Framework.Diagnostics
{
    public static class FileNames
    {
        // Test names can hold characters a file name cannot; each run of them becomes one underscore.
        public static string Sanitise(string name)
        {
            return string.Join("_", name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
