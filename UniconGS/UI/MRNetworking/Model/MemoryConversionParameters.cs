
namespace UniconGS.UI.MRNetworking.Model

{
    public class MemoryConversionParameters
    {

        public MemoryConversionParameters()
        {
            MaximumOfUshortValue = ushort.MaxValue;
            NumberOfSigns = 1;
            LimitOfValue = 100; 
        }
        #region Implementation of IMemoryConversion

        public int LimitOfValue { get; set; }

        public int MaximumOfUshortValue { get; set; }

        public int NumberOfSigns { get; set; }

        #endregion
    }
}
