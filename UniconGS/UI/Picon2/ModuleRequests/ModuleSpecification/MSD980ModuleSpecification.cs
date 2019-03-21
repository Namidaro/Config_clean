using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification
{
    public class MSD980ModuleSpecification : IModuleSpecification
    {
        public string ModuleName
        {
            get { return "МСД980"; }
        }
        public byte ModuleType
        {
            get { return 0x08; }
        }
        public byte Command
        {
            get { return 0x20; }
        }
        public ushort ModuleParameterAddress
        {
            get { return 0x0000; }
        }
        public ushort FirstModuleDatabaseAddress
        {
            get { return 0x2020; }
        }
        public byte ParameterCount
        {
            get { return 0x01; }
        }

        public MSD980ModuleSpecification()
        {
                
        }
    }
}
