﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PowerShellTools.Common.ServiceManagement.ExplorerContract;

namespace PowerShellTools
{
    [CallbackBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false,
        IncludeExceptionDetailInFaults = true)]
    [DebugServiceEventHandlerBehavior]
    public class ExplorerServiceEventsHandlerProxy : IPowerShellExplorerServiceCallback
    {
    }
}
