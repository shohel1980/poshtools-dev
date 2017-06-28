using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace PowerShellTools.Classification
{
    /// <summary>
    /// Classifier Provider that exposes the classification of PowerShell scripts via
    /// the PowerShell classifier.
    /// </summary>
    [ContentType("PowerShell"), Export(typeof(IClassifierProvider))]
    internal sealed class PowerShellClassifierProvider : IClassifierProvider
    {
        [Export]
        [Name("PowerShell")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition PowerShellContentType = null;

        [Export]
        [FileExtension(".psd1")]
        [ContentType("PowerShell")]
        internal static FileExtensionToContentTypeDefinition Psd1 = null;

        [Export]
        [FileExtension(".psm1")]
        [ContentType("PowerShell")]
        internal static FileExtensionToContentTypeDefinition Psm1 = null;

        [Export]
        [FileExtension(".ps1")]
        [ContentType("PowerShell")]
        internal static FileExtensionToContentTypeDefinition Ps1 = null;

        [Import]
        private IDependencyValidator _validator = null;

        [Import]
        public IClassificationFormatMapService ClassificationFormatMapService { get; set; }

        [Import]
        public IClassificationTypeRegistryService ClassificationTypeRegistryService { get; set; }


        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (!_validator.Validate()) return null;

            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new PowerShellClassifier(textBuffer));
        }
    }
}
