using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UniconGS.UI.MRNetworking.Model;

namespace UniconGS.UI.MRNetworking.ViewModel
{
    public class ModbusMemorySettingsViewModel :BindableBase
    {
        private int _baseAdress;
        private int _numberOfPoints;
        private ModbusMemorySettings _modbusMemorySettings;
        private ObservableCollection<int> _numberOfPointsCollection;

        public ModbusMemorySettingsViewModel()
        {
            NumberOfPointsCollection = new ObservableCollection<int>() { 8, 12, 16, 32 };
            NumberOfPoints = NumberOfPointsCollection[1];
            AddressStepDownCommand = new DelegateCommand(OnAddressStepDownExecute);
            AddressStepUpCommand = new DelegateCommand(OnAddressStepUpExecute);
            _modbusMemorySettings=new ModbusMemorySettings();
        }

        private void OnAddressStepUpExecute()
        {
            if (int.MaxValue - _baseAdress > NumberOfPoints)
            {
                _baseAdress += NumberOfPoints;
            }
            else
            {
                _baseAdress = int.MaxValue - NumberOfPoints;
            }
            RaisePropertyChanged(nameof(BaseAdressHex));
            RaisePropertyChanged(nameof(BaseAdressDec));
            ModbusMemorySettingsChanged?.Invoke(GetModbusMemorySettings());

        }

        private void OnAddressStepDownExecute()
        {
            if (NumberOfPoints < _baseAdress)
            {
                _baseAdress -= NumberOfPoints;
            }
            else
            {
                _baseAdress = 0;
            }
            RaisePropertyChanged(nameof(BaseAdressHex));
            RaisePropertyChanged(nameof(BaseAdressDec));
            ModbusMemorySettingsChanged?.Invoke(GetModbusMemorySettings());
        }



        #region Implementation of IModbusMemorySettingsViewModel

        public ICommand AddressStepDownCommand { get; set; }

        public string BaseAdressDec
        {
            get { return _baseAdress.ToString(); }
            set
            {
                _baseAdress = int.Parse(value);
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(BaseAdressHex));
                ModbusMemorySettingsChanged?.Invoke(GetModbusMemorySettings());

            }
        }

        public string BaseAdressHex
        {
            get { return _baseAdress.ToString("X"); }
            set
            {
                try
                {
                    _baseAdress = int.Parse(value, NumberStyles.HexNumber);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(BaseAdressDec));
                    ModbusMemorySettingsChanged?.Invoke(GetModbusMemorySettings());
                }
                catch(Exception ex)
                {

                }
            }
        }

        public int NumberOfPoints
        {
            get { return _numberOfPoints; }
            set
            {
                _numberOfPoints = value;
                RaisePropertyChanged();
                ModbusMemorySettingsChanged?.Invoke(GetModbusMemorySettings());

            }
        }


        public ObservableCollection<int> NumberOfPointsCollection
        {
            get { return _numberOfPointsCollection; }
            set
            {
                _numberOfPointsCollection = value;
                RaisePropertyChanged();
            }
        }

        public Action<ModbusMemorySettings> ModbusMemorySettingsChanged { get; set; }

        #endregion



        public ModbusMemorySettings GetModbusMemorySettings()
        {
            if (_modbusMemorySettings == null) return null;
            _modbusMemorySettings.BaseAdress = _baseAdress;
            _modbusMemorySettings.NumberOfPoints = NumberOfPoints;
            return _modbusMemorySettings;
        }

        public ICommand AddressStepUpCommand { get; set; }
    }
}
