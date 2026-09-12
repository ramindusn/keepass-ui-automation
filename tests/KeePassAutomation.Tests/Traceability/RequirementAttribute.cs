using System;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;

namespace KeePassAutomation.Tests.Traceability
{
    // Names the requirement a test verifies. In the report it is the test's feature, so the
    // Behaviors view groups tests by requirement, and its description, so the test's entry in the report says
    // what the test is for. The coverage check on CI reads the feature label.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequirementAttribute : AllureTestCaseAttribute
    {
        private readonly string _id;
        private readonly string _description;

        public RequirementAttribute(string id, string description)
        {
            _id = id;
            _description = description;
        }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.labels.Add(Label.Feature(_id + " " + _description));
            testResult.description = _id + ": " + _description;
        }
    }
}
