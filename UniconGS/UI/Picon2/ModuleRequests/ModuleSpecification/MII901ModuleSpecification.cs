using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification
{
    public class MII901ModuleSpecification:IModuleSpecification
    {
        public string DeviceName
        {
            get { return "МИИ901"; }
        }
        public byte ModuleType
        {
            get { return 0x05; }
        }
        public byte Command
        {
            get
            {
                return 0x20;
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
                return 0x0E;
            }
        }
    }
}
