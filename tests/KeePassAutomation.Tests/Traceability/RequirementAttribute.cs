using System;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;

namespace KeePassAutomation.Tests.Traceability
{
    // Names the requirement a test verifies. The report groups tests by it and shows it on each test,
    // and the coverage check on CI reads it.
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
