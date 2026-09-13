using Allure.NUnit.Attributes;

namespace KeePassAutomation.Tests
{
    // The requirement a test verifies: its wording, and the number of its GitHub issue.
    public class RequirementAttribute : AllureTmsAttribute
    {
        public RequirementAttribute(string wording, int issue) : base(wording, issue.ToString())
        {
        }
    }
}
