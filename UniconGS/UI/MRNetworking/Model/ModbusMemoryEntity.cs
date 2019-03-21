using System;

namespace UniconGS.UI.MRNetworking.Model
    
{
    public class ModbusMemoryEntity : ICloneable
    {
        private ushort _value;
        private MemoryConversionParameters _memoryConversionParameters;


        #region Implementation of IModbusMemoryEntity

        public void SetUshortValue(ushort value)
        {
            _value = value;
        }

        public void SetConversion(MemoryConversionParameters memoryConversionParameters)
        {
            _memoryConversionParameters = memoryConversionParameters;
        }

        public int Adress { get; set; }

        public int DirectValue => _value;
        
        public string ConvertedValue => ((_memoryConversionParameters.LimitOfValue * _value) /
                                         _memoryConversionParameters.MaximumOfUshortValue)
            .ToString("N" + _memoryConversionParameters.NumberOfSigns);

      

        #endregion

        #region Implementation of ICloneable

        public object Clone()
        {
            ModbusMemoryEntity clone=new ModbusMemoryEntity();
            clone.Adress = Adress;
            clone.SetUshortValue(_value);
            clone.SetConversion(_memoryConversionParameters);
            return clone;
        }

        #endregion
    }
}