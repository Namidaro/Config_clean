using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification
{
    public class MRV980ModuleSpecification : IModuleSpecification
    {
        public string DeviceName
        {
            get { return "МРВ980"; }
        }
        public byte ModuleType
        {
            get { return 0x0B; }
        }
        public byte Command
        {
            get
            {
                return 0x40;
            }
        }
        public ushort ModuleParameterAddress
        {
            get
            {
                return 0x0000;
            }
        }
        public ushort FirstModuleDatabaseAddress
        {
            get
            {
                return 0x2000;
            }
        }
        public byte ParameterCount
        {
            get
            {
                return 0x01;
            }
        }
        public MRV980ModuleSpecification()
        {

        }
    }
}
