using Prism.Mvvm;

namespace UniconGS.UI.MRNetworking.Model
{
  public class MemoryBitViewModel:BindableBase
    {
        private bool? _boolValue;
        private int _bitNumber;

        #region Implementation of IMemoryBitViewModel

      
        public bool? BoolValue
        {
            get { return _boolValue; }
            set
            {
                _boolValue = value;
                RaisePropertyChanged();
            }
        }

        public int BitNumber
        {
            get { return _bitNumber; }
            set
            {
                _bitNumber = value; 
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
