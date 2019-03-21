using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniconGS.UI.MRNetworking.Model;

namespace UniconGS.UI.MRNetworking.ViewModel
{
    public class ModbusMemoryEntityViewModel :BindableBase,ICloneable
    {     

        private ModbusMemoryEntity _modbusMemoryEntity;
        private ModbusConversionParametersViewModel _modbusConversionParametersViewModel;
        private int _address;
        private List<bool> _bits;
        private string _convertedValue;
        private string _directValueHex;
        private string _directValueDec;
        private bool _isError;
        private List<MemoryBitViewModel> _memoryBitViewModels;
        public ModbusMemoryEntityViewModel(ModbusConversionParametersViewModel modbusConversionParametersViewModel)
        {
            _modbusConversionParametersViewModel = modbusConversionParametersViewModel;
            _modbusMemoryEntity=new ModbusMemoryEntity();
            _isError = true;
            _memoryBitViewModels = new List<MemoryBitViewModel>(16);
            _modbusMemoryEntity.SetConversion(_modbusConversionParametersViewModel.GetConversionParameters());

        }

        private void OnMemoryConversionParametersChanged(MemoryConversionParameters memoryConversionParameters)
        {
            _modbusMemoryEntity.SetConversion(memoryConversionParameters);
            RaisePropertyChanged(nameof(ConvertedValue));

        }


        #region Implementation of IModbusMemoryEntityViewModel

        public void SetAddress(int address)
        {
            _address = address;
            RaisePropertyChanged(nameof(AdressDec));
            RaisePropertyChanged(nameof(AdressHex));

        }

        public ModbusConversionParametersViewModel ModbusConversionParametersViewModel
        {
            get { return _modbusConversionParametersViewModel; }
            set
            {
                try
                {
                    if (_modbusConversionParametersViewModel != null)
                        if (_modbusConversionParametersViewModel.MemoryConversionParametersChanged != null)
                        {
                            _modbusConversionParametersViewModel.MemoryConversionParametersChanged = null;

                        }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                _modbusConversionParametersViewModel = value;
                _modbusConversionParametersViewModel.MemoryConversionParametersChanged +=
                    OnMemoryConversionParametersChanged;
                RaisePropertyChanged();
            }
        }

        public string AdressHex
        {
            get { return _address.ToString("X"); }
        }

        public string AdressDec
        {
            get { return _address.ToString("D"); }
        }


        public string ConvertedValue => _modbusMemoryEntity.ConvertedValue;

        public string DirectValueHex => _modbusMemoryEntity.DirectValue.ToString("X");

        public string DirectValueDec => _modbusMemoryEntity.DirectValue.ToString("D");

        public void SetUshortValue(ushort value)
        {
            _isError = false;
            _modbusMemoryEntity.SetUshortValue(value);
            RaisePropertyChanged(nameof(Bits));
            RaisePropertyChanged(nameof(ConvertedValue));
            RaisePropertyChanged(nameof(DirectValueHex));
            RaisePropertyChanged(nameof(DirectValueDec));

        }

        public List<MemoryBitViewModel> Bits
        {
            //get
            //{
            //    bool[] bitBools = new bool[16];
            //    //BitArray bitArray = new BitArray(new int[] {_modbusMemoryEntity.DirectValue});

            //    for (int i = 0; i < 16; i++)
            //    {
            //        bitBools[15 - i] = ((_modbusMemoryEntity.DirectValue >> i) & 1) == 1;
            //    }

            //    for (int i = 0; i < 16; i++)
            //    {
            //        if (_memoryBitViewModels.Count > i)
            //        {
            //            _memoryBitViewModels[i].BitNumber = i;
            //            if (_isError)
            //                _memoryBitViewModels[i].BoolValue = null;
            //            else
            //                _memoryBitViewModels[i].BoolValue = bitBools[i];
            //            continue;
            //        }
            //        MemoryBitViewModel memoryBitViewModel = new MemoryBitViewModel();
            //        memoryBitViewModel.BitNumber = 15 - i;
            //        if (_isError)
            //            memoryBitViewModel.BoolValue = null;
            //        else
            //            memoryBitViewModel.BoolValue = bitBools[i];


            //        _memoryBitViewModels.Add(memoryBitViewModel);
            //    }
            //    return _memoryBitViewModels;
            //}


            get
            {
                BitArray bitArray = new BitArray(new int[] { _modbusMemoryEntity.DirectValue });
                for (int i = 0; i < 16; i++)
                {
                    if (_memoryBitViewModels.Count > i)
                    {
                        _memoryBitViewModels[i].BitNumber =15- i;
                        if (_isError)
                            _memoryBitViewModels[i].BoolValue = null;
                        else
                            _memoryBitViewModels[i].BoolValue = bitArray[15-i];
                        continue;
                    }

                    MemoryBitViewModel memoryBitViewModel = new MemoryBitViewModel();
                    memoryBitViewModel.BitNumber =15- i;
                    if (_isError)
                        memoryBitViewModel.BoolValue = null;
                    else
                        memoryBitViewModel.BoolValue = bitArray[15-i];


                    _memoryBitViewModels.Add(memoryBitViewModel);
                }

                return _memoryBitViewModels;
            }
        }

        public void SetError()
        {
            _isError = true;
            _modbusMemoryEntity.SetUshortValue(0);
            RaisePropertyChanged(nameof(ConvertedValue));
            RaisePropertyChanged(nameof(DirectValueHex));
            RaisePropertyChanged(nameof(DirectValueDec));
            RaisePropertyChanged(nameof(Bits));

        }

        #endregion

        #region Implementation of ICloneable

        public object Clone()
        {
            ModbusMemoryEntityViewModel memoryEntityViewModel = new ModbusMemoryEntityViewModel(_modbusConversionParametersViewModel);
            memoryEntityViewModel.SetUshortValue((ushort)_modbusMemoryEntity.DirectValue);
            memoryEntityViewModel.SetAddress(_address);
            return memoryEntityViewModel;

        }

        #endregion
    }
}