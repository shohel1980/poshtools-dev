using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace PowerShellTools.MSBuild
{
    public class RunPowerShellCommand : Task
    {
        [Required]
        public ITaskItem Command { get; set; }
        
        public override bool Execute()
        {
            var host = new MSBuildPowerShellHost(s =>
            {
                if (!string.IsNullOrEmpty(s))
                    this.Log.LogMessage(MessageImportance.High, s);
            });

            try
            {
                var runspace = RunspaceFactory.CreateRunspace(host);
                runspace.Open();

                var pipe = runspace.CreatePipeline();
                pipe.Commands.AddScript(Command.ItemSpec);
                pipe.Commands.Add("out-default");
                pipe.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                pipe.Invoke();
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }

            return true;
        }
    }
}
