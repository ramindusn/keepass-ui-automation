using System;
using System.IO;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Framework.AppUnderTest
{
    public static class KeePassPackage
    {
        // Points at any KeePass.exe, for running the suite against an installed copy or another build.
        private const string ExecutableVariable = "KEEPASS_EXE";

        // The pinned version and its checksum live in tools/Get-KeePass.ps1, and nowhere else.
        public static string FindExecutable()
        {
            var overridePath = Environment.GetEnvironmentVariable(ExecutableVariable);

            if (!string.IsNullOrEmpty(overridePath))
            {
                if (!File.Exists(overridePath))
                {
                    throw new InvalidOperationException(
                        ExecutableVariable + " is set to " + overridePath + ", but there is no such file.");
                }

                return overridePath;
            }

            var path = Path.Combine(RepositoryRoot.Locate(), ".keepass", "KeePass.exe");

            if (!File.Exists(path))
            {
                throw new InvalidOperationException(
                    "KeePass is not present at " + path + ". Run: pwsh tools/Get-KeePass.ps1, or set " + ExecutableVariable + ".");
            }

            return path;
        }
    }
}
