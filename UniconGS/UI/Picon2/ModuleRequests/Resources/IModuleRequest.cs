using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.Resources
{
    interface IModuleRequest
    {
        byte Period { get; set; }
        byte Type { get; set; }
        byte CrateAddress { get; set; }
        byte Command { get; set; }
        byte[] ParameterModuleAddress { get; set; }
        byte[] ParameterBaseAddress { get; set; }
        byte ParameterCount { get; set; }
        ushort[] Request { get; set; }
        string UIRequest { get; set; }
    }
}
