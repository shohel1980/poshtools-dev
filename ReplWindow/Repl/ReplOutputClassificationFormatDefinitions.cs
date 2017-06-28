﻿/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace PowerShellTools.Repl {
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]    
    [UserVisible(true)]
    internal class InteractiveBlackFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - Black";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveBlackFormatDefinition() {
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveDarkRedFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - DarkRed";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveDarkRedFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x7f, 0, 0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveDarkGreenFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - DarkGreen";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveDarkGreenFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x00, 0x7f, 0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveDarkYellowFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - DarkYellow";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveDarkYellowFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x7f, 0x7f, 0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveDarkBlueFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - DarkBlue";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveDarkBlueFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x00, 0x00, 0x7f);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveDarkMagentaFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - DarkMagenta";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveDarkMagentaFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x7f, 0x00, 0x7f);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveDarkCyanFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - DarkCyan";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveDarkCyanFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x00, 0x7f, 0x7f);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveGrayFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - Gray";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveGrayFormatDefinition() {
            ForegroundColor = Color.FromRgb(0xC0, 0xC0, 0xC0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveDarkGrayFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - DarkGray";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF

        public InteractiveDarkGrayFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x7f, 0x7f, 0x7f);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveRedFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - Red";
        
        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveRedFormatDefinition() {
            ForegroundColor = Color.FromRgb(0xff, 0, 0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveGreenFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - Green";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveGreenFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x00, 0xff, 0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveYellowFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - Yellow";
        
        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF

        public InteractiveYellowFormatDefinition() {
            ForegroundColor = Color.FromRgb(0xff, 0xff, 0);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    [Order(After = Priority.Default, Before = Priority.High)]
    internal class InteractiveBlueFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - Blue";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveBlueFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x00, 0x00, 0xff);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveMagentaFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - Magenta";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveMagentaFormatDefinition() {
            ForegroundColor = Color.FromRgb(0xff, 0x00, 0xff);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveCyanFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - Cyan";
        
        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF

        public InteractiveCyanFormatDefinition() {
            ForegroundColor = Color.FromRgb(0x00, 0xff, 0xff);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [DisplayName(Name)]
    [UserVisible(true)]
    internal class InteractiveWhiteFormatDefinition : ClassificationFormatDefinition {
        public const string Name = "PowerShell Interactive - White";

        [Export]
        [Name(Name)]
        [BaseDefinition(PredefinedClassificationTypeNames.NaturalLanguage)]
        internal static ClassificationTypeDefinition Definition = null; // Set via MEF
        
        public InteractiveWhiteFormatDefinition() {
            // not really white by default so the user can actually see "white" text.
            ForegroundColor = Color.FromRgb(0x7f, 0x7f, 0x7f);
        }
    }
}
