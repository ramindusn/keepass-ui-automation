namespace KeePassAutomation.Tests.Setup
{
    public enum VideoMode
    {
        Off,

        // Every test leaves a video, named <Test>.Passed.mp4 or <Test>.Failed.mp4.
        On,

        // Every test is recorded, but the video of a passed test is deleted.
        RetainOnFailure
    }
}
