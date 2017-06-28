using System;

namespace PowerShellTools
{
    internal static class GuidList
    {
        public const string PowerShellLanguage = "1C4711F1-3766-4F84-9516-43FA4169CC36";

        //Package Guids; these need to be the same as the guids in PowerShellTools.vsct/Common.Build.Targets
#if DEV15
        public const string PowerShellToolsPackageGuid = "0429083f-fdbc-47a3-84ff-b3d50343b21e";
        public const string PowerShellToolsProjectPackageGuid = "1C9F35A6-C599-4336-A6F7-A9A9AC9A2B86";
#elif DEV14
        public const string PowerShellToolsPackageGuid = "59875F69-67B7-4A5C-B33A-9E2C2B5D266D";
        public const string PowerShellToolsProjectPackageGuid = "5520558A-236B-453B-8CFF-442671DB0BB5";
#elif DEV12
        public const string PowerShellToolsPackageGuid = "58dce676-42b0-4dd6-9ee4-afbc8e582b8a";
        public const string PowerShellToolsProjectPackageGuid = "2F99237E-E34F-4A3D-A337-500E4B3167B8";
#else
#error "No Visual Studio Version Provided"
#endif

        //Property Pages
        public const string GeneralPropertiesPageGuid = "C9619BDD-D1B3-4ACA-ADF3-2323EB62315E";
        public const string DebugPropertiesPageGuid = "E6C81F79-910C-4C91-B9DF-321883DC9F44";
        public const string BuildEventsPropertiesPageGuid = "0974F0EF-8D19-423E-9637-FB853C4EA232";
        public const string ModuleManifestPropertiesPageGuid = "3B19A81A-BE20-4F0D-B577-5093853FB681";
        public const string AdvancedPropertiesPageGuid = "1B8CADD5-EC43-474F-90FD-C0BD7EB78C8E";

        //Commands
        public const string CmdSetGuid = "099073C0-B561-4BC1-A847-92657B89A00E";
        public const uint CmdidExecuteSelection = 0x0102;
        public const uint CmdidExecuteAsScript =  0x0103;
        public const uint CmdidExecuteWithParametersAsScript = 0x0104;
        public const uint CmdidGotoDefinition = 0x0105;
        public const uint CmdidSnippet = 0x0106;
        public const uint CmdidPrettyPrint = 0x0107;
        public const uint CmdidDisplayRepl = 0x0108;
        public const uint CmdidExecuteAsScriptSolution = 0x0109;
        public const uint CmdidExecuteWithParametersAsScriptFromSolutionExplorer = 0x0110;
        public const uint CmdidDisplayExplorer = 0x0111;

        public const string guidCustomEditorCmdSetString = "73d661d7-c0a7-476c-ad5e-3b36f1e91a8f";
        public const string guidCustomEditorEditorFactoryString = "0ff6321c-6ea5-400b-8342-f126da8505a2";

        public static readonly Guid guidCustomEditorCmdSet = new Guid(guidCustomEditorCmdSetString);
        public static readonly Guid guidCustomEditorEditorFactory = new Guid(guidCustomEditorEditorFactoryString);
    };
}