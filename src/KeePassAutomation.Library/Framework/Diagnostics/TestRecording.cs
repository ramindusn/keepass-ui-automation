using System;
using System.IO;
using FlaUI.Core.Capturing;
using FlaUI.Core.Tools;

namespace KeePassAutomation.Framework.Diagnostics
{
    // A video of one test: FlaUI takes screenshots and ffmpeg encodes them.
    public sealed class TestRecording : IDisposable
    {
        private readonly VideoRecorder _recorder;
        private bool _finished;

        private TestRecording(VideoRecorder recorder, string filePath)
        {
            _recorder = recorder;
            FilePath = filePath;
        }

        public string FilePath { get; }

        public static TestRecording Start(string ffmpegPath, string targetPath)
        {
            var directory = Path.GetDirectoryName(targetPath);

            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            var settings = new VideoRecorderSettings
            {
                ffmpegPath = ffmpegPath,
                TargetVideoPath = targetPath,
                FrameRate = 5,
                VideoFormat = VideoFormat.x264,
                // FlaUI's default of 0 is lossless x264, which makes huge files.
                VideoQuality = 26,
                UseCompressedImages = true,
            };

            // The whole screen, so dialogs outside the main window are recorded too.
            return new TestRecording(new VideoRecorder(settings, _ => Capture.Screen()), targetPath);
        }

        // Stops recording and waits until ffmpeg has written and released the file.
        public string Finish(TimeSpan timeout)
        {
            Dispose();

            if (!Retry.WhileFalse(() => IsReleased(FilePath), timeout).Success)
            {
                throw new IOException("The video at " + FilePath + " was not finished within "
                    + timeout.TotalSeconds.ToString("0") + "s.");
            }

            return FilePath;
        }

        public void Dispose()
        {
            if (_finished)
            {
                return;
            }

            _finished = true;
            _recorder.Stop();
            _recorder.Dispose();
        }

        private static bool IsReleased(string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return stream.Length > 0;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
