using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniconGS.UI.MRNetworking.Model;

namespace UniconGS.UI.MRNetworking.ViewModel
{
    public class ModbusConversionParametersViewModel : BindableBase
    {
        

        private int _limitOfValue;
        private int _maximumOfUshortValue;
        private int _numberOfSigns;
        private ObservableCollection<int> _numberOfSignsCollection;
        private MemoryConversionParameters _memoryConversionParameters;

        public ModbusConversionParametersViewModel()
        {
            _memoryConversionParameters=new MemoryConversionParameters();
            _maximumOfUshortValue = _memoryConversionParameters.MaximumOfUshortValue;
            _numberOfSigns = _memoryConversionParameters.NumberOfSigns;
            _limitOfValue = _memoryConversionParameters.LimitOfValue;
        }


        

        #region Implementation of IModbusConversionParametersViewModel

        public int LimitOfValue
        {
            get { return _limitOfValue; }
            set
            {
                _limitOfValue = value;
                RaisePropertyChanged();
                MemoryConversionParametersChanged?.Invoke(GetConversionParameters());
            }
        }

        public int MaximumOfUshortValue
        {
            get { return _maximumOfUshortValue; }
            set
            {
                _maximumOfUshortValue = value;
                RaisePropertyChanged();
                MemoryConversionParametersChanged?.Invoke(GetConversionParameters());

            }
        }

        public int NumberOfSigns
        {
            get { return _numberOfSigns; }
            set
            {
                _numberOfSigns = value;
                RaisePropertyChanged();
                MemoryConversionParametersChanged?.Invoke(GetConversionParameters());

            }
        }


        public Action<MemoryConversionParameters> MemoryConversionParametersChanged { get; set; }

        #endregion


        public MemoryConversionParameters GetConversionParameters()
        {
            _memoryConversionParameters.LimitOfValue = LimitOfValue;
            _memoryConversionParameters.MaximumOfUshortValue = MaximumOfUshortValue;
            _memoryConversionParameters.NumberOfSigns = NumberOfSigns;
            return _memoryConversionParameters;
        }


    }
}
