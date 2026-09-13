using System;
using System.IO;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Framework.AppUnderTest
{
    public static class KeePassPackage
    {
        // Always the copy fetched into .keepass, so every run tests the same pinned version.
        // The version and its checksum live in tools/Get-KeePass.ps1, and nowhere else.
        public static string FindExecutable()
        {
            var path = Path.Combine(RepositoryRoot.Locate(), ".keepass", "KeePass.exe");

            if (!File.Exists(path))
            {
                throw new InvalidOperationException(
                    "KeePass is not present at " + path + ". Run: pwsh tools/Get-KeePass.ps1");
            }

            return path;
        }
    }
}
