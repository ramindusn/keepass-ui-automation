using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace KeePassAutomation.Tests.Traceability
{
    // NUnit writes the requirement into the result XML, which the traceability matrix is generated from.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequirementAttribute : NUnitAttribute, IApplyToTest
    {
        public RequirementAttribute(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; }

        public string Description { get; }

        public void ApplyToTest(Test test)
        {
            test.Properties.Add("Requirement", Id);
            test.Properties.Add("RequirementDescription", Description);
        }
    }
}
