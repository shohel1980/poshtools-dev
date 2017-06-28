﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudioTools;
using Microsoft.VisualStudioTools.Navigation;
using Microsoft.Win32;
using PowerShellTools.Classification;
using PowerShellTools.Commands;
using PowerShellTools.Common.ServiceManagement.DebuggingContract;
using PowerShellTools.Common.ServiceManagement.IntelliSenseContract;
using PowerShellTools.Contracts;
using PowerShellTools.DebugEngine;
using PowerShellTools.Diagnostics;
using PowerShellTools.Intellisense;
using PowerShellTools.LanguageService;
using PowerShellTools.Options;
using PowerShellTools.Project.PropertyPages;
using PowerShellTools.Service;
using PowerShellTools.ServiceManagement;
using Engine = PowerShellTools.DebugEngine.Engine;
using MessageBox = System.Windows.MessageBox;
using Threading = System.Threading.Tasks;
using PowerShellTools.Common.Logging;
using PowerShellTools.DebugEngine.Remote;
using PowerShellTools.Explorer;
using PowerShellTools.Common.ServiceManagement.ExplorerContract;

namespace PowerShellTools
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    //[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]

    // There are a few user scenarios which will trigger package to load
    // 1. Open/Create any type of PowerShell project
    // 2. Open/Create PowerShell file(.ps1, .psm1, .psd1) from file->open/create or solution explorer
    // 3. Execute PowerShell script file from solution explorer
    [ProvideAutoLoad(PowerShellTools.Common.Constants.PowerShellProjectUiContextString)]
    // 4. PowerShell interactive window open
    [ProvideAutoLoad(PowerShellTools.Common.Constants.PowerShellReplCreationUiContextString)]
    // 5. PowerShell service execution
    [ProvideService(typeof(IPowerShellService))]
    [ProvideService(typeof(IPowerShellHostClientService))]
    [ProvideLanguageService(typeof(PowerShellLanguageInfo),
                            PowerShellConstants.LanguageName,
                            101,
                            ShowSmartIndent = true,
                            ShowDropDownOptions = true,
                            EnableCommenting = true,
                            RequestStockColors = true)]
    [ProvideEditorFactory(typeof(PowerShellEditorFactory), 114, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
    [ProvideBraceCompletion(PowerShellConstants.LanguageName)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideKeyBindingTable(GuidList.guidCustomEditorEditorFactoryString, 102)]
    [Guid(GuidList.PowerShellToolsPackageGuid)]
    [ProvideObject(typeof(AdvancedPropertyPage))]
    [ProvideObject(typeof(ModuleManifestPropertyPage))]
    [ProvideObject(typeof(GeneralPropertyPage))]
    [ProvideObject(typeof(DebugPropertyPage))]
    [ProvideObject(typeof(BuildEventPropertyPage))]
    [Microsoft.VisualStudio.Shell.ProvideDebugEngine("{43ACAB74-8226-4920-B489-BFCF05372437}", "PowerShell",
        PortSupplier = "{708C1ECA-FF48-11D2-904F-00C04FA302A1}",
        ProgramProvider = "{08F3B557-C153-4F6C-8745-227439E55E79}", Attach = true,
        CLSID = "{C7F9F131-53AB-4FD0-8517-E54D124EA392}")]
    [Clsid(Clsid = "{C7F9F131-53AB-4FD0-8517-E54D124EA392}",
           Assembly = "PowerGuiVsx.Core.DebugEngine",
        Class = "PowerGuiVsx.Core.DebugEngine.Engine")]
    [Clsid(Clsid = "{08F3B557-C153-4F6C-8745-227439E55E79}",
           Assembly = "PowerGuiVsx.Core.DebugEngine",
        Class = "PowerGuiVsx.Core.DebugEngine.ScriptProgramProvider")]
    [Microsoft.VisualStudioTools.ProvideDebugEngine("PowerShell",
                                                    typeof(ScriptProgramProvider),
                                                    typeof(Engine),
        "{43ACAB74-8226-4920-B489-BFCF05372437}")]
    [ProvideIncompatibleEngineInfo("{92EF0900-2251-11D2-B72E-0000F87572EF}")]
    [ProvideIncompatibleEngineInfo("{F200A7E7-DEA5-11D0-B854-00A0244A1DE2}")]
    [ProvideOptionPage(typeof(GeneralDialogPage), PowerShellConstants.LanguageDisplayName, "General", 101, 106, true)]
    [ProvideOptionPage(typeof(DiagnosticsDialogPage), PowerShellConstants.LanguageDisplayName, "Diagnostics", 101, 106, true)]
    [ProvideDiffSupportedContentType(".ps1;.psm1;.psd1", ";")]
    [ProvideLanguageExtension(typeof(PowerShellLanguageInfo), ".ps1")]
    [ProvideLanguageExtension(typeof(PowerShellLanguageInfo), ".psm1")]
    [ProvideLanguageExtension(typeof(PowerShellLanguageInfo), ".psd1")]
    [ProvideCodeExpansions(GuidList.PowerShellLanguage, false, 106, "PowerShell", @"Snippets\SnippetsIndex.xml", @"Snippets\PowerShell\")]
    [ProvideDebugPortSupplier("Powershell Remote Debugging (SSL Required)", typeof(RemoteDebugPortSupplier), PowerShellTools.Common.Constants.PortSupplierId, typeof(RemotePortPicker))]
    [ProvideDebugPortSupplier("Powershell Remote Debugging", typeof(RemoteUnsecuredDebugPortSupplier), PowerShellTools.Common.Constants.UnsecuredPortSupplierId, typeof(RemoteUnsecuredPortPicker))]
    [ProvideDebugPortPicker(typeof(RemotePortPicker))]
    [ProvideToolWindow(
        typeof(PSCommandExplorerWindow),
        Style = Microsoft.VisualStudio.Shell.VsDockStyle.Tabbed,
        Window = "dd9b7693-1385-46a9-a054-06566904f861")]
    public sealed class PowerShellToolsPackage : CommonPackage
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PowerShellToolsPackage));
        private Lazy<PowerShellService> _powerShellService;
        private Lazy<PowerShellHostClientService> _powerShellHostClientService;
        private static ScriptDebugger _debugger;
        private ITextBufferFactoryService _textBufferFactoryService;
        private static Dictionary<ICommand, MenuCommand> _commands;
        private IContentType _contentType;
        private IntelliSenseEventsHandlerProxy _intelliSenseServiceContext;
        private static PowerShellToolsPackage _instance;
        public static EventWaitHandle DebuggerReadyEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        public static bool PowerShellHostInitialized = false;
	    public bool ResetPowerShellSession;
	    private SolutionEventsListener _solutionEventsListener;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public PowerShellToolsPackage()
        {
            _commands = new Dictionary<ICommand, MenuCommand>();
            DependencyValidator = new DependencyValidator();
        }

        /// <summary>
        /// Returns the current package instance.
        /// </summary>
        internal static PowerShellToolsPackage Instance
        {
            get
            {
                if (_instance == null)
                {
                    ThreadHelper.Generic.Invoke(() =>
                    {
                        if (_instance == null)
                        {
                            var vsShell = Package.GetGlobalService(typeof(SVsShell)) as IVsShell;
                            var packageGuid = new Guid(GuidList.PowerShellToolsPackageGuid);
                            IVsPackage vsPackage;
                            if (vsShell.IsPackageLoaded(ref packageGuid, out vsPackage) != VSConstants.S_OK)
                            {
                                vsShell.LoadPackage(ref packageGuid, out vsPackage);
                            }

                            _instance = (PowerShellToolsPackage)vsPackage;
                        }
                    });
                }

                return _instance;
            }
        }

        public static IPowerShellDebuggingService DebuggingService
        {
            get
            {
                return ConnectionManager.Instance.PowerShellDebuggingService;
            }
        }

        public IntelliSenseEventsHandlerProxy IntelliSenseServiceContext
        {
            get
            {
                return _intelliSenseServiceContext;
            }
        }

        public new object GetService(Type type)
        {
            return base.GetService(type);
        }

        public override Type GetLibraryManagerType()
        {
            return null;
        }

        public override bool IsRecognizedFile(string filename)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the PowerShell host for the package.
        /// </summary>
        internal static ScriptDebugger Debugger
        {
            get
            {
                return _debugger;
            }
        }

        /// <summary>
        /// Indicate if override the execution policy
        /// </summary>
        internal static bool OverrideExecutionPolicyConfiguration { get; private set; }

        internal static IPowerShellIntelliSenseService IntelliSenseService
        {
            get
            {
                return ConnectionManager.Instance.PowerShellIntelliSenseSerivce;
            }
        }

        public static IPowerShellExplorerService ExplorerService
        {
            get
            {
                return ConnectionManager.Instance.PowerShellExplorerService;
            }
        }

        internal DependencyValidator DependencyValidator { get; set; }

        internal override LibraryManager CreateLibraryManager(CommonPackage package)
        {
            throw new NotImplementedException();
        }

        internal IContentType ContentType
        {
            get
            {
                if (_contentType == null)
                {
                    _contentType = ComponentModel.GetService<IContentTypeRegistryService>().GetContentType(PowerShellConstants.LanguageName);
                }
                return _contentType;
            }
        }

        internal T GetDialogPage<T>() where T : DialogPage
        {
            return (T)GetDialogPage(typeof(T));
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            try
            {
                EnvDTE.DTE dte = (EnvDTE.DTE)GetGlobalService(typeof(EnvDTE.DTE));
                Log.InfoFormat("PowerShell Tools Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
                Log.InfoFormat("Visual Studio Version: {0}", dte.Version);
                Log.InfoFormat("Windows Version: {0}", Environment.OSVersion);
                Log.InfoFormat("Current Culture: {0}", CultureInfo.CurrentCulture);
                if (!DependencyValidator.Validate())
                {
                    Log.Warn("Dependency check failed.");
                    return;
                }

                base.Initialize();

                InitializeInternal();

                _powerShellService = new Lazy<PowerShellService>(() => { return new PowerShellService(); });
                _powerShellHostClientService = new Lazy<PowerShellHostClientService> (() => { return new PowerShellHostClientService(); });
                
                RegisterServices();

	            if (ShouldShowReleaseNotes())
	            {
					IVsWindowFrame ppFrame;
		            var service = GetGlobalService(typeof(IVsWebBrowsingService)) as IVsWebBrowsingService;
		            service.Navigate("https://poshtools.com/release/current", (uint)__VSWBNAVIGATEFLAGS.VSNWB_ForceNew, out ppFrame);
				}

	            
			}
            catch (Exception ex)
            {
                Log.Error("Failed to initialize package.", ex);
                MessageBox.Show(
                    Resources.PowerShellToolsInitializeFailed + ex,
                    Resources.MessageBoxErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

	    private bool ShouldShowReleaseNotes()
	    {
			var page = (GeneralDialogPage)GetDialogPage(typeof(GeneralDialogPage));
		    if (!page.ShowReleaseNotes) return false;

		    using (var key = Registry.CurrentUser.CreateSubKey(@"Software\PowerShell Tools for Visual Studio"))
		    {
			    var lastReleaseNoteVersion = key.GetValue("LastReleaseNoteVersion") as string;
			    if (string.IsNullOrEmpty(lastReleaseNoteVersion))
			    {
				    key.SetValue("LastReleaseNoteVersion", Version);
				    return true;
			    }

			    if (System.Version.Parse(lastReleaseNoteVersion) < System.Version.Parse(Version))
			    {
					key.SetValue("LastReleaseNoteVersion", Version);
				    return true;
				}

			    return false;
		    }
	    }

	    public static string Version
	    {
		    get
		    {
			    Assembly asm = Assembly.GetExecutingAssembly();
			    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
			    return fvi.FileVersion;
		    }
	    }

		private void InitializeInternal()
        {
            _intelliSenseServiceContext = new IntelliSenseEventsHandlerProxy();

            var diagnosticsPage = (DiagnosticsDialogPage)GetDialogPage(typeof(DiagnosticsDialogPage));

            if (diagnosticsPage.EnableDiagnosticLogging)
            {
                DiagnosticConfiguration.EnableDiagnostics();
            }

            Log.Info(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));

            var langService = new PowerShellLanguageInfo(this);
            ((IServiceContainer)this).AddService(langService.GetType(), langService, true);

            _textBufferFactoryService = ComponentModel.GetService<ITextBufferFactoryService>();

            if (_textBufferFactoryService != null)
            {
                _textBufferFactoryService.TextBufferCreated += TextBufferFactoryService_TextBufferCreated;
            }

            var textManager = (IVsTextManager)GetService(typeof(SVsTextManager));
            var adaptersFactory = ComponentModel.GetService<IVsEditorAdaptersFactoryService>();

            RefreshCommands(new ExecuteSelectionCommand(this.DependencyValidator),
                            new ExecuteFromEditorContextMenuCommand(this.DependencyValidator),
                            new ExecuteWithParametersAsScriptCommand(adaptersFactory, textManager, this.DependencyValidator),
                            new ExecuteFromSolutionExplorerContextMenuCommand(this.DependencyValidator),
                            new ExecuteWithParametersAsScriptFromSolutionExplorerCommand(adaptersFactory, textManager, this.DependencyValidator),
                            new PrettyPrintCommand(),
                            new OpenDebugReplCommand(),
                            new OpenExplorerCommand());

            try
            {
                Threading.Task.Run(
                    () =>
                    {
                        InitializePowerShellHost();
                    }
                );
            }
            catch (AggregateException ae)
            {
                Log.Error("Failed to initalize PowerShell host.", ae.Flatten());
                MessageBox.Show(
                    Resources.PowerShellHostInitializeFailed,
                    Resources.MessageBoxErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                throw ae.Flatten();
            }
        }

        internal void ShowExplorerWindow()
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(PSCommandExplorerWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("");
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        //private void ShowToolWindow(object sender, EventArgs e)
        //{
        //    // Get the instance number 0 of this tool window. This window is single instance so this instance
        //    // is actually the only one.
        //    // The last flag is set to true so that if the tool window does not exists it will be created.
        //    ToolWindowPane window = this.FindToolWindow(typeof(PSCommandExplorerWindow), 0, true);
        //    if ((null == window) || (null == window.Frame))
        //    {
        //        throw new NotSupportedException("");
        //    }
        //    IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
        //    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        //}

        private void RefreshCommands(params ICommand[] commands)
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                foreach (var command in commands)
                {
                    var menuCommand = new OleMenuCommand(command.Execute, command.CommandId);
                    menuCommand.BeforeQueryStatus += command.QueryStatus;
                    mcs.AddCommand(menuCommand);
                    _commands[command] = menuCommand;
                }
            }
        }

        /// <summary>
        /// Register Services
        /// </summary>
        private void RegisterServices()
        {
            Debug.Assert(this is IServiceContainer, "The package is expected to be an IServiceContainer.");

            var serviceContainer = (IServiceContainer)this;
            serviceContainer.AddService(typeof(IPowerShellService), (c, t) => _powerShellService.Value, true);
            serviceContainer.AddService(typeof(IPowerShellHostClientService), (c, t) => _powerShellHostClientService.Value, true);
        }

        private void TextBufferFactoryService_TextBufferCreated(object sender, TextBufferCreatedEventArgs e)
        {
            ITextBuffer buffer = e.TextBuffer;

            buffer.ContentTypeChanged += TextBuffer_ContentTypeChanged;

            EnsureBufferHasTokenizer(e.TextBuffer.ContentType, buffer);
        }

        private void TextBuffer_ContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            var buffer = sender as ITextBuffer;

            Debug.Assert(buffer != null, "buffer is null");

            EnsureBufferHasTokenizer(e.AfterContentType, buffer);
        }

        private void EnsureBufferHasTokenizer(IContentType contentType, ITextBuffer buffer)
        {
            if (contentType.IsOfType(PowerShellConstants.LanguageName) && !buffer.Properties.ContainsProperty(BufferProperties.PowerShellTokenizer))
            {
                IPowerShellTokenizationService psts = new PowerShellTokenizationService(buffer);

                buffer.PostChanged += (o, args) => psts.StartTokenization();

                buffer.Properties.AddProperty(BufferProperties.PowerShellTokenizer, psts);
            }
        }

	    /// <summary>
	    /// Initialize the PowerShell host.
	    /// </summary>
	    private void InitializePowerShellHost()
	    {
		    var page = (GeneralDialogPage) GetDialogPage(typeof(GeneralDialogPage));

		    ResetPowerShellSession = page.ResetPowerShellSession;
		    OverrideExecutionPolicyConfiguration = page.OverrideExecutionPolicyConfiguration;

		    Log.Info("InitializePowerShellHost");

		    _debugger = new ScriptDebugger(page.OverrideExecutionPolicyConfiguration);

		    // Warm up the intellisense service due to the reason that the 
		    // first intellisense request is often times slower than usual
		    // TODO: Should we move this into the HostService's initializiation?
		    IntelliSenseService.GetDummyCompletionList();

		    DebuggerReadyEvent.Set();

		    PowerShellHostInitialized = true;

		    if (page.ShouldLoadProfiles)
		    {
			    DebuggingService.LoadProfiles();
		    }

		    SetReplLocationToSolutionDir();
			_solutionEventsListener = new SolutionEventsListener(this);
			_solutionEventsListener.StartListeningForChanges();
			_solutionEventsListener.SolutionOpened += _solutionEventsListener_SolutionOpened;
        }

		private void _solutionEventsListener_SolutionOpened(object sender, EventArgs e)
		{
			SetReplLocationToSolutionDir();
		}

	    private void SetReplLocationToSolutionDir()
	    {
			var solution = GetService(typeof(IVsSolution)) as IVsSolution;
		    if (solution != null)
		    {
			    string solutionDir, solutionFile, other;
			    if (solution.GetSolutionInfo(out solutionDir, out solutionFile, out other) == VSConstants.S_OK)
				    Debugger.ExecuteInternal(string.Format("Set-Location '{0}'", solutionDir));
		    }
		}

		internal void BitnessSettingChanged(object sender, BitnessEventArgs e)
        {
            ConnectionManager.Instance.ProcessEventHandler(e.NewBitness);
        }

	    internal void ResetPowerShellSessionChanged(object sender, EventArgs<bool> e)
	    {
		    ResetPowerShellSession = e.Value;
	    }

        internal void DiagnosticLoggingSettingChanged(object sender, bool enabled)
        {
            if (sender is DiagnosticsDialogPage)
            {
                if (enabled)
                {
                    DiagnosticConfiguration.EnableDiagnostics();
                }
                else
                {
                    DiagnosticConfiguration.DisableDiagnostics();
                }
            }
        }
    }
}
