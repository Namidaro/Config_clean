using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using UniconGS.UI.MRNetworking.ViewModel;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using UniconGS.Source;
using UniconGS.UI.MRNetworking.Model;

namespace UniconGS.UI.MRNetworking.ViewModel
{
    public class ModbusEntityEditingViewModel : BindableBase
        {
        private ModbusMemoryEntityViewModel _modbusMemoryEntityViewModelToEdit;
        private ushort _resultedValueUshort;
        private string _valueDec;
        private string _valueHex;
            private ushort _addressToWrite;


        public ModbusEntityEditingViewModel()
        {
            WriteCommand = new DelegateCommand<object>(OnExecuteWrite);
            ChangeBitValueCommand = new DelegateCommand<int?>(OnExecuteChangeBitValue);
        }

        private void OnExecuteChangeBitValue(int? bitNumber)
        {
            if (!bitNumber.HasValue) throw new ArgumentException();
            var bools = new bool[16];
            foreach (MemoryBitViewModel memoryBitViewModel in ModbusMemoryEntityViewModelToEdit.Bits)
            {
                bools[memoryBitViewModel.BitNumber] = memoryBitViewModel.BoolValue.Value;
            }
            bools[bitNumber.Value] = !bools[bitNumber.Value];
            ushort resultedValue = 0;
            for (int i = 0; i < bools.Count(); i++)
            {
                if (bools[i])
                {
                    _resultedValueUshort = resultedValue += (ushort)(1 << i);
                }
            }
            ModbusMemoryEntityViewModelToEdit.SetUshortValue(resultedValue);
            ValueDec = ModbusMemoryEntityViewModelToEdit.DirectValueDec;
            ValueHex = ModbusMemoryEntityViewModelToEdit.DirectValueHex;
        }



        private async void OnExecuteWrite(object obj)
        {
            if (await WriteContext())
            {
                if (obj is Window)
                {
                    ((Window)obj).Close();
                }
            }
        }


        #region Implementation of IModbusEntityEditingViewModel

        public async void SetEntity(ModbusMemoryEntityViewModel modbusMemoryEntityViewModelToEdit)
        {
            _modbusMemoryEntityViewModelToEdit = modbusMemoryEntityViewModelToEdit;
            RaisePropertyChanged(nameof(ModbusMemoryEntityViewModelToEdit));
            ValueDec = ModbusMemoryEntityViewModelToEdit.DirectValueDec;
            ValueHex = ModbusMemoryEntityViewModelToEdit.DirectValueHex;
            RaisePropertyChanged(nameof(ModbusMemoryEntityViewModelToEdit.AdressDec));
            RaisePropertyChanged(nameof(ModbusMemoryEntityViewModelToEdit.AdressHex));
          _addressToWrite = ushort.Parse(ModbusMemoryEntityViewModelToEdit.AdressDec);
           _resultedValueUshort = ushort.Parse(ModbusMemoryEntityViewModelToEdit.DirectValueDec);
       //  await WriteContext(_resultedValueUshort);



            //Querer = new Slot(ushort.Parse(ModbusMemoryEntityViewModelToEdit.AdressDec), 1, "MBNetworkQuerer");

        }

        public ModbusMemoryEntityViewModel ModbusMemoryEntityViewModelToEdit
        {
            get { return _modbusMemoryEntityViewModelToEdit; }
        }

        public ICommand WriteCommand { get; }


        public ICommand ChangeBitValueCommand { get; }

        public string ValueHex
        {
            get { return _valueHex; }
            set
            {

                //валидация
                //FireErrorsChanged(nameof(ValueHex));
                //if (HasErrors)
                //{
                //    _valueHex = ModbusMemoryEntityViewModelToEdit.DirectValueHex;
                //    return;
                //}


                try
                {
                    ModbusMemoryEntityViewModelToEdit.SetUshortValue(Convert.ToUInt16(value, 16));
                }
                catch (Exception e)
                {
                    return;
                }
                _valueHex = value;
                RaisePropertyChanged();

                if (ValueDec != ModbusMemoryEntityViewModelToEdit.DirectValueDec)
                {
                    ValueDec = ModbusMemoryEntityViewModelToEdit.DirectValueDec;
                }

                RaisePropertyChanged(nameof(ValueDec));
                _resultedValueUshort = ushort.Parse(ModbusMemoryEntityViewModelToEdit.DirectValueDec);
            }
        }

        public string ValueDec
        {
            get { return _valueDec; }
            set
            {
                try
                {
                    ModbusMemoryEntityViewModelToEdit.SetUshortValue(Convert.ToUInt16(value));
                }
                catch (Exception e)
                {
                    return;
                }
                _valueDec = value;
                //FireErrorsChanged(nameof(ValueDec));
                //if (HasErrors)
                //{
                //    if (HasErrors)
                //    {
                //        _valueDec = ModbusMemoryEntityViewModelToEdit.DirectValueDec;
                //        return;
                //    }
                //}
                ModbusMemoryEntityViewModelToEdit.SetUshortValue(Convert.ToUInt16(value));
                RaisePropertyChanged();
                if (ValueHex != ModbusMemoryEntityViewModelToEdit.DirectValueHex)
                {
                    ValueHex = ModbusMemoryEntityViewModelToEdit.DirectValueHex;
                }
                RaisePropertyChanged(nameof(ValueHex));
                _resultedValueUshort = ushort.Parse(ModbusMemoryEntityViewModelToEdit.DirectValueDec);

            }
        }

        #endregion


        //#region Overrides of ValidatableBindableBase

        //protected override void OnValidate()
        //{
        //    var res = new ModbusEntityEditingViewModelValidator(_localizerService).Validate(this);
        //    SetValidationErrors(res);

        //}

        //#endregion

        #region Implementation of IQuery

        //public Slot Querer
        //{
        //    get { return _querer; }
        //    set { _querer = value; }
        //}

        public ushort[] Value { get; set; }

        public void Update()
        {
            throw new NotImplementedException();
        }



        public async Task<bool> WriteContext()
        {
            bool res = false;
            Value = new[] { _resultedValueUshort };
            try
            {
                Value = this.Value;
               
                    Value = this.Value;
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, _addressToWrite, Value );
                res = true;
            }
            catch
            {
            }

            return res;
        }

        #endregion
    }
}
