using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace PowerShellTools.TestAdapter
{
	public class TestCaseSet
	{
		private List<TestResult> _testResults;

		public TestCaseSet(string fileName, string describe)
		{
			File = fileName;
			Describe = describe;
			TestCases = new List<TestCase>();
		}

		public string File { get; }
		public string Describe { get; }
		public List<TestCase> TestCases { get; }
		public IEnumerable<TestResult> TestResults { get { return _testResults; } }

		public void ProcessTestResults(Array results)
		{
			_testResults = new List<TestResult>();

			foreach (PSObject result in results)
			{
				var describe = result.Properties["Describe"].Value as string;
				if (!HandleParseError(result, describe))
				{
					break;
				}

				var context = result.Properties["Context"].Value as string;
				var name = result.Properties["Name"].Value as string;

				if (string.IsNullOrEmpty(context))
					context = "No Context";

				// Skip test cases we aren't trying to run
				var testCase = TestCases.FirstOrDefault(m => m.FullyQualifiedName == string.Format("{0}.{1}.{2}", describe, context, name));
				if (testCase == null) continue;
				var testResult = new TestResult(testCase);

				testResult.Outcome = GetOutcome(result.Properties["Result"].Value as string);

				var stackTraceString = result.Properties["StackTrace"].Value as string;
				var errorString = result.Properties["FailureMessage"].Value as string;

				testResult.ErrorStackTrace = stackTraceString;
				testResult.ErrorMessage = errorString;

				_testResults.Add(testResult);
			}
		}

		private bool HandleParseError(PSObject result, string describe)
		{
			var errorMessage = string.Format("Error in {0}", File);
			if (describe.Contains(errorMessage))
			{
				var stackTraceString = result.Properties["StackTrace"].Value as string;
				var errorString = result.Properties["FailureMessage"].Value as string;

				foreach (var tc in TestCases)
				{
					var testResult = new TestResult(tc);
					testResult.Outcome = TestOutcome.Failed;
					testResult.ErrorMessage = errorString;
					testResult.ErrorStackTrace = stackTraceString;
					_testResults.Add(testResult);
				}

				return false;
			}

			return true;
		}

		private static TestOutcome GetOutcome(string testResult)
		{
			if (string.IsNullOrEmpty(testResult))
			{
				return TestOutcome.NotFound;
			}

			if (testResult.Equals("passed", StringComparison.OrdinalIgnoreCase))
			{
				return TestOutcome.Passed;
			}
			if (testResult.Equals("skipped", StringComparison.OrdinalIgnoreCase))
			{
				return TestOutcome.Skipped;
			}
			if (testResult.Equals("pending", StringComparison.OrdinalIgnoreCase))
			{
				return TestOutcome.Skipped;
			}
			return TestOutcome.Failed;
		}
	}
}
