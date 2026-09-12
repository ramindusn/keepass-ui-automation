using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;

namespace KeePassAutomation.MatrixGenerator
{
    public static class Program
    {
        // Returns 1 when a requirement has no test, so CI fails on an uncovered requirement.
        public static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine("Usage: MatrixGenerator <requirements.json> <result1.xml> [result2.xml ...] <output.md>");
                return 1;
            }

            var requirementsPath = args[0];
            var outputPath = args[args.Length - 1];

            var resultPaths = new List<string>();

            for (var i = 1; i < args.Length - 1; i++)
            {
                resultPaths.Add(args[i]);
            }

            var requirements = ReadRequirements(requirementsPath);
            var rows = ReadResults(resultPaths);

            return WriteMatrix(requirements, rows, outputPath);
        }

        private static List<RequirementDefinition> ReadRequirements(string requirementsPath)
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var json = File.ReadAllText(requirementsPath);
            var requirements = JsonSerializer.Deserialize<List<RequirementDefinition>>(json, jsonOptions);

            if (requirements == null)
            {
                throw new InvalidOperationException("Could not read requirements from " + requirementsPath + ".");
            }

            return requirements;
        }

        private static List<TestResultRow> ReadResults(List<string> resultPaths)
        {
            var rows = new List<TestResultRow>();

            foreach (var resultPath in resultPaths)
            {
                var doc = XDocument.Load(resultPath);
                var runAt = ReadRunAt(doc);

                foreach (var testCase in doc.Descendants("test-case"))
                {
                    var requirementId = ReadRequirementId(testCase);

                    if (requirementId == null)
                    {
                        continue;
                    }

                    rows.Add(new TestResultRow(requirementId, ReadTestName(testCase), ReadResult(testCase), runAt));
                }
            }

            return rows;
        }

        private static string ReadRunAt(XDocument doc)
        {
            if (doc.Root == null)
            {
                return "unknown";
            }

            return ReadAttribute(doc.Root, "start-time", "unknown");
        }

        // The requirement id is written into the result XML by RequirementAttribute.
        private static string ReadRequirementId(XElement testCase)
        {
            var properties = testCase.Element("properties");

            if (properties == null)
            {
                return null;
            }

            foreach (var property in properties.Elements("property"))
            {
                var name = property.Attribute("name");
                var value = property.Attribute("value");

                if (name != null && value != null && name.Value == "Requirement")
                {
                    return value.Value;
                }
            }

            return null;
        }

        private static string ReadTestName(XElement testCase)
        {
            var fullName = testCase.Attribute("fullname");

            if (fullName != null)
            {
                return fullName.Value;
            }

            return ReadAttribute(testCase, "name", "unknown");
        }

        private static string ReadResult(XElement testCase)
        {
            return ReadAttribute(testCase, "result", "Unknown");
        }

        private static string ReadAttribute(XElement element, string name, string fallback)
        {
            var attribute = element.Attribute(name);

            if (attribute == null)
            {
                return fallback;
            }

            return attribute.Value;
        }

        private static List<TestResultRow> MatchesFor(List<TestResultRow> rows, string requirementId)
        {
            var matches = new List<TestResultRow>();

            foreach (var row in rows)
            {
                if (row.RequirementId == requirementId)
                {
                    matches.Add(row);
                }
            }

            return matches;
        }

        private static int WriteMatrix(List<RequirementDefinition> requirements, List<TestResultRow> rows, string outputPath)
        {
            var lines = new List<string>
            {
                "# Traceability matrix",
                "",
                "Generated from test results. Do not edit by hand.",
                "",
                "| Requirement | Test | Result | Run at |",
                "|---|---|---|---|"
            };

            var uncovered = new List<RequirementDefinition>();

            foreach (var requirement in requirements)
            {
                var matches = MatchesFor(rows, requirement.Id);

                if (matches.Count == 0)
                {
                    uncovered.Add(requirement);
                    lines.Add("| " + requirement.Id + " | **NO TEST FOUND** | — | — |");
                    continue;
                }

                foreach (var match in matches)
                {
                    lines.Add("| " + requirement.Id + " | " + match.TestName + " | " + match.Result
                        + " | " + match.RunAt + " |");
                }
            }

            lines.Add("");

            if (uncovered.Count == 0)
            {
                lines.Add("All " + requirements.Count + " requirements have at least one test.");
            }
            else
            {
                var ids = new List<string>();

                foreach (var requirement in uncovered)
                {
                    ids.Add(requirement.Id);
                }

                lines.Add("**" + uncovered.Count + " requirement(s) have no test:** " + string.Join(", ", ids));
            }

            File.WriteAllLines(outputPath, lines);

            Console.WriteLine("Wrote " + outputPath + ": " + rows.Count + " test result(s) across "
                + requirements.Count + " requirement(s), " + uncovered.Count + " uncovered.");

            if (uncovered.Count == 0)
            {
                return 0;
            }

            return 1;
        }
    }
}
