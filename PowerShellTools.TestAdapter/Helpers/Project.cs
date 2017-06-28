using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;

namespace PowerShellTools.TestAdapter.Helpers
{
	public class Project : IProject
	{
		public Project(IVsProject project)
		{
			Items = VsSolutionHelper.GetProjectItems(project);
		}
		public IEnumerable<string> Items { get; }
	}
}
