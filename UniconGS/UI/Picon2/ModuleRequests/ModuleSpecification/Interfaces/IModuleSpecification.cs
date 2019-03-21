using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification
{
    interface IModuleSpecification
    {
        byte ModuleType { get; }
        byte Command { get; }
        ushort ModuleParameterAddress { get; }
        ushort FirstModuleDatabaseAddress { get; }
        byte ParameterCount { get; }

    }
}
