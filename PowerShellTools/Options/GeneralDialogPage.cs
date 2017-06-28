﻿using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using PowerShellTools.DebugEngine;

namespace PowerShellTools.Options
{
    internal class GeneralDialogPage : DialogPage
    {
        private BitnessOptions _savedBitness;
	    private bool _savedResetSession;

        public event EventHandler<BitnessEventArgs> BitnessSettingChanged;
	    public event EventHandler<EventArgs<bool>> ResetPowerShellSessionChanged;

		/// <summary>
		/// The constructor
		/// </summary>
		public GeneralDialogPage()
        {
            InitializeSettings();
        }

        [DisplayName(@"Enable Unrestricted Execution Policy")]
        [Description("This setting controls the execution policy for executing PowerShell scripts in Visual Studio. True, will set the Visual Studio process execution policy to \"Unrestricted\".  False, will use the current user or local machine policy.")]
        public bool OverrideExecutionPolicyConfiguration { get; set; }

        [DisplayName(@"Multiline REPL Window")]
        [Description("When false, pressing enter invokes the command line in the REPL Window rather than starting a new line.")]
        public bool MultilineRepl { get; set; }

        [Browsable(true)]
        [DisplayName(@"Bitness")]
        [Description("This setting controls the bitness of PowerShell execution within Visual Studio.")]
        public BitnessOptions Bitness { get; set; }

        [DisplayName(@"Load Profiles on Start")]
        [Description("When false, the host service will not load any profiles on startup.")]
        public bool ShouldLoadProfiles { get; set; }

	    [DisplayName(@"Reset PowerShell Session")]
	    [Description("When true, resets the PowerShell session between executions.")]
	    public bool ResetPowerShellSession { get; set; }

	    [DisplayName(@"Show Release Notes")]
	    [Description("When true, shows release notes after PowerShell tools is updated.")]
	    public bool ShowReleaseNotes { get; set; }

		protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            // On a non-64bit machine, bitness cannot be changed.
            if (!Environment.Is64BitOperatingSystem)
            {
                Bitness = BitnessOptions.DefaultToOperatingSystem;
                return;
            }

	        if (_savedResetSession != ResetPowerShellSession)
	        {
				var changed = ResetPowerShellSessionChanged;
		        if (changed != null)
		        {
			        changed(this, new EventArgs<bool>(ResetPowerShellSession));
		        }
			}

	        _savedResetSession = ResetPowerShellSession;

            if (_savedBitness != Bitness)
            {
                var changed = BitnessSettingChanged;
                if (changed != null)
                {
                    changed(this, new BitnessEventArgs(_savedBitness));
                }
            }
            _savedBitness = Bitness;
        }

        /// <summary>
        /// These are the default settings for the dialog's options. This is called before LoadSettingsFromStorage/Xml, so the below will be overridden if persisted settings exist.
        /// </summary>
        private void InitializeSettings()
        {
            this.OverrideExecutionPolicyConfiguration = true;

            this.MultilineRepl = false;
            if (Environment.Is64BitOperatingSystem)
            {
                _savedBitness = Bitness;
            }
            else
            {
                Bitness = BitnessOptions.DefaultToOperatingSystem;
            }

	        ResetPowerShellSessionChanged += PowerShellToolsPackage.Instance.ResetPowerShellSessionChanged;
            BitnessSettingChanged += PowerShellToolsPackage.Instance.BitnessSettingChanged;

	        this.ResetPowerShellSession = false;
            this.ShouldLoadProfiles = true;
	        this.ShowReleaseNotes = true;
        }
    }
}
