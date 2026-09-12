using System;
using System.IO;

namespace KeePassAutomation.Framework.Core
{
    // Walks up from the test assembly to the solution file, where .keepass/ and .ffmpeg/ live.
    public static class RepositoryRoot
    {
        private const string SolutionFile = "KeePassAutomation.sln";

        public static string Locate()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null && !File.Exists(Path.Combine(directory.FullName, SolutionFile)))
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new InvalidOperationException("Could not locate the repository root (" + SolutionFile + " not found).");
            }

            return directory.FullName;
        }
    }
}
