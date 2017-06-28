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

using Microsoft.VisualStudio.Text;

namespace PowerShellTools.Repl {
#if INTERACTIVE_WINDOW
    using IReplEvaluator = IInteractiveEngine;
#elif POWERSHELL
    using IReplEvaluator = IPowerShellReplEvaluator;
#endif

    public static class ReplWindowExtensions {
        public static IReplEvaluator GetReplEvaluator(this ITextBuffer textBuffer) {
            IReplEvaluator res;
            if (textBuffer.Properties.TryGetProperty<IReplEvaluator>(typeof(IReplEvaluator), out res)) {
                return res;
            }
            return null;
        }
    }
}
