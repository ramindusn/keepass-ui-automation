using System;
using Allure.NUnit.Attributes;

namespace KeePassAutomation.Tests.Traceability
{
    // Names the requirement a test verifies. Allure groups tests by feature, so the report's
    // Behaviors view lists every requirement with the tests that cover it and their results.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequirementAttribute : AllureFeatureAttribute
    {
        public RequirementAttribute(string id, string description) : base(id + " " + description)
        {
        }
    }
}
