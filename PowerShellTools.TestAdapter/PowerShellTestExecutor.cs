using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.PowerShell;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using PowerShellTools.TestAdapter.Properties;
using System.Collections.ObjectModel;

namespace PowerShellTools.TestAdapter
{
    [ExtensionUri(ExecutorUriString)]
    public class PowerShellTestExecutor : ITestExecutor
    {
        public void RunTests(IEnumerable<string> sources, IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            SetupExecutionPolicy();
            IEnumerable<TestCase> tests = PowerShellTestDiscoverer.GetTests(sources, null);
            RunTests(tests, runContext, frameworkHandle);
        }

        private static void SetupExecutionPolicy()
        {
            SetExecutionPolicy(ExecutionPolicy.RemoteSigned, ExecutionPolicyScope.Process);
        }

        private static void SetExecutionPolicy(ExecutionPolicy policy, ExecutionPolicyScope scope)
        {
            ExecutionPolicy currentPolicy = ExecutionPolicy.Undefined;

            using (var ps = PowerShell.Create())
            {
                ps.AddCommand("Get-ExecutionPolicy");

                foreach (var result in ps.Invoke())
                {
                    currentPolicy = ((ExecutionPolicy)result.BaseObject);
                    break;
                }

                if ((currentPolicy <= policy || currentPolicy == ExecutionPolicy.Bypass) && currentPolicy != ExecutionPolicy.Undefined) //Bypass is the absolute least restrictive, but as added in PS 2.0, and thus has a value of '4' instead of a value that corresponds to it's relative restrictiveness
                    return;

                ps.Commands.Clear();

                ps.AddCommand("Set-ExecutionPolicy").AddParameter("ExecutionPolicy", policy).AddParameter("Scope", scope).AddParameter("Force");
                ps.Invoke();
            }
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _mCancelled = false;
            SetupExecutionPolicy();

	        var testSets = new List<TestCaseSet>();
	        foreach (var testCase in tests)
	        {
		        var describe = testCase.FullyQualifiedName.Split('.').First();
		        var codeFile = testCase.CodeFilePath;

		        var testSet = testSets.FirstOrDefault(m => m.Describe.Equals(describe, StringComparison.OrdinalIgnoreCase) &&
		                                                   m.File.Equals(codeFile, StringComparison.OrdinalIgnoreCase));

		        if (testSet == null)
		        {
			        testSet = new TestCaseSet(codeFile, describe);
					testSets.Add(testSet);
		        }

				testSet.TestCases.Add(testCase);
	        }

            foreach (var testSet in testSets)
            {
                if (_mCancelled) break;

                var testOutput = new StringBuilder();

                try
                {
                    var testAdapter = new TestAdapterHost();
                    testAdapter.HostUi.OutputString = s =>
                    {
						if (!string.IsNullOrEmpty(s))
							testOutput.Append(s);
                    };

                    var runpsace = RunspaceFactory.CreateRunspace(testAdapter);
                    runpsace.Open();

                    using (var ps = PowerShell.Create())
                    {
                        ps.Runspace = runpsace;
	                    RunTestSet(ps, testSet, runContext);

	                    foreach (var testResult in testSet.TestResults)
	                    {
							frameworkHandle.RecordResult(testResult);
						}
                    }
                }
                catch (Exception ex)
                {
	                foreach (var testCase in testSet.TestCases)
	                {
						var testResult = new TestResult(testCase);
		                testResult.Outcome = TestOutcome.Failed;
		                testResult.ErrorMessage = ex.Message;
		                testResult.ErrorStackTrace = ex.StackTrace;
						frameworkHandle.RecordResult(testResult);
	                }
                }

                if (testOutput.Length > 0)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, testOutput.ToString());
                }
            }
        }

        public void Cancel()
        {
            _mCancelled = true;
        }

        public const string ExecutorUriString = "executor://PowerShellTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
        private bool _mCancelled;

	    public void RunTestSet(PowerShell powerShell, TestCaseSet testCaseSet, IRunContext runContext)
	    {
			SetupExecutionPolicy();
		    var module = FindModule("Pester", runContext);
		    powerShell.AddCommand("Import-Module").AddParameter("Name", module);
		    powerShell.Invoke();
		    powerShell.Commands.Clear();

		    if (powerShell.HadErrors)
		    {
			    var errorRecord = powerShell.Streams.Error.FirstOrDefault();
			    var errorMessage = errorRecord == null ? string.Empty : errorRecord.ToString();

				throw new Exception(Resources.FailedToLoadPesterModule + errorMessage);
		    }

		    var fi = new FileInfo(testCaseSet.File);

		    powerShell.AddCommand("Invoke-Pester")
			    .AddParameter("Path", fi.Directory.FullName)
			    .AddParameter("TestName", testCaseSet.Describe)
			    .AddParameter("PassThru");

		    var pesterResults = powerShell.Invoke();
		    powerShell.Commands.Clear();

		    // The test results are not necessary stored in the first PSObject.
		    var results = GetTestResults(pesterResults);
			testCaseSet.ProcessTestResults(results);
		}

        protected string FindModule(string moduleName, IRunContext runContext)
        {
            var pesterPath = GetModulePath(moduleName, runContext.TestRunDirectory);
            if (string.IsNullOrEmpty(pesterPath))
            {
                pesterPath = GetModulePath(moduleName, runContext.SolutionDirectory);
            }

            if (string.IsNullOrEmpty(pesterPath))
            {
                pesterPath = moduleName;
            }

            return pesterPath;
        }

        /// <summary>
        /// Gets test results from the <see cref="PSObject"/> collection.
        /// </summary>
        /// <param name="psObjects">
        /// The <see cref="PSObject"/> collection as returned from the <c>Invoke-Pester</c> command
        /// </param>
        /// <returns>
        /// The test results as <see cref="Array"/>
        /// </returns>
        private static Array GetTestResults(Collection<PSObject> psObjects)
        {
            var resultObject = psObjects.FirstOrDefault(o => o.Properties["TestResult"] != null);

            return resultObject.Properties["TestResult"].Value as Array;
        }

        private static string GetModulePath(string moduleName, string root)
        {
            if (root == null)
                return null;
            
            // Default packages path for nuget.
            var packagesRoot = Path.Combine(root, "packages");

            // TODO: Scour for custom nuget packages paths.

            if (Directory.Exists(packagesRoot))
            {
                var packagePath = Directory.GetDirectories(packagesRoot, moduleName + "*", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (null != packagePath)
                {
                    var psd1 = Path.Combine(packagePath, string.Format(@"tools\{0}.psd1", moduleName));
                    if (File.Exists(psd1))
                    {
                        return psd1;
                    }

                    var psm1 = Path.Combine(packagePath, string.Format(@"tools\{0}.psm1", moduleName));
                    if (File.Exists(psm1))
                    {
                        return psm1;
                    }
                    var dll = Path.Combine(packagePath, string.Format(@"tools\{0}.dll", moduleName));
                    if (File.Exists(dll))
                    {
                        return dll;
                    }
                }
            }

            return null;
        }
    }

}
