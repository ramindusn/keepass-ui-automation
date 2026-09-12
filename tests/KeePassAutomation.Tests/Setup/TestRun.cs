using KeePassAutomation.Tests.Setup;
using NUnit.Framework;

// Outside a namespace on purpose: NUnit applies a SetUpFixture to the whole assembly, so this runs
// once for the run rather than once per test.
[SetUpFixture]
public class TestRun
{
    [OneTimeSetUp]
    public void BeforeAll()
    {
        if (!TestConfig.Run.WriteManifest)
        {
            return;
        }

        // On CI a run that cannot describe itself is not evidence; locally it is only a warning.
        try
        {
            RunManifest.Write();
        }
        catch (System.Exception ex)
        {
            if (TestConfig.IsCi)
            {
                throw;
            }

            TestContext.WriteLine("Could not write the run manifest: " + ex.Message);
        }
    }
}
