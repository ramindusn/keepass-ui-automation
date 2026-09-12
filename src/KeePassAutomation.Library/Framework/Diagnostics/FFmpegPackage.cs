using System.IO;
using KeePassAutomation.Framework.Core;

namespace KeePassAutomation.Framework.Diagnostics
{
    public static class FFmpegPackage
    {
        // Null when ffmpeg is missing; the caller decides whether that is fatal (it is on CI).
        public static string TryFindExecutable()
        {
            var path = Path.Combine(RepositoryRoot.Locate(), ".ffmpeg", "ffmpeg.exe");

            if (!File.Exists(path))
            {
                return null;
            }

            return path;
        }
    }
}
