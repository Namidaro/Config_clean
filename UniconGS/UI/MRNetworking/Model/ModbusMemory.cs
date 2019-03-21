using System.Collections.Generic;

namespace UniconGS.UI.MRNetworking.Model
{
    public class ModbusMemory
    {

        #region Implementation of IModbusMemory

        public List<ModbusMemoryEntity> CurrentValues { get; set; }
        
        #endregion

    }
}
