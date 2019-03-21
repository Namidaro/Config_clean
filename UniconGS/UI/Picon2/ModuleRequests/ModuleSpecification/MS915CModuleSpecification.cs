using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification
{
    public class MS915CModuleSpecification : IModuleSpecification
    {
        public string DeviceName
        {
            get { return "Счетчик"; }
        }
        public byte ModuleType
        {
            get { return 0x0E; }
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
                return 0x001C;
            }
        }
        public byte ParameterCount
        {
            get
            {
                return 0x1E;
            }
        }
        public MS915CModuleSpecification()
        {

        }
    }
}
