using Microsoft.Practices.ObjectBuilder2;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Prism.Commands;
using UniconGS.Interfaces;
using UniconGS.Source;
using UniconGS.UI.MRNetworking.Model;
using UniconGS.UI.Settings;
using Application = System.Windows.Application;
using Timer = System.Threading.Timer;

namespace UniconGS.UI.MRNetworking.ViewModel
{
    public class ModbusMemoryViewModel : BindableBase
    {
        #region Private fields

        private ObservableCollection<ModbusMemoryEntityViewModel> _modbusMemoryEntityViewModels;
        private ModbusMemorySettingsViewModel _modbusMemorySettingsViewModel;
        private bool _isQueriesStarted;
        private bool _isQueriesAllowed;
        private List<ModbusConversionParametersViewModel> _modbusConversionParametersViewModels;
        private string _nameForUi;
        public event ControllerSettings.ShowMessageEventHandler ShowMessage;
        private Timer _queryTimer;
        private SemaphoreSlim _queriesSemaphoreSlim;
        #endregion

        #region C-tor 

        public ModbusMemoryViewModel()
        {
            ModbusMemoryEntityViewModels = new ObservableCollection<ModbusMemoryEntityViewModel>();
            _modbusConversionParametersViewModels = new List<ModbusConversionParametersViewModel>(32);
            ModbusMemorySettingsViewModel = new ModbusMemorySettingsViewModel();
            OnModbusMemorySettingsChanged(ModbusMemorySettingsViewModel.GetModbusMemorySettings());
            EditEntityCommand = new DelegateCommand<ModbusMemoryEntityViewModel>(OnExecuteEditEntity);
            _queryTimer = new Timer(OnQueryTimerTriggered, null, 1000, 1000);
            _queriesSemaphoreSlim = new SemaphoreSlim(1, 1);
            IsQueriesAllowed = false;
        }

        private async void OnQueryTimerTriggered(object state)
        {
            if (!IsQueriesAllowed) return;
            if (!_isQueriesStarted) return;
            if (_queriesSemaphoreSlim.CurrentCount == 0) return;
            await _queriesSemaphoreSlim.WaitAsync();
            await Update();
            if (_queriesSemaphoreSlim.CurrentCount == 0)
            {
                _queriesSemaphoreSlim.Release(1);
            }
        }


        private void OnExecuteEditEntity(ModbusMemoryEntityViewModel modbusMemoryEntityViewModel)
        {

            ModbusEntityEditingViewModel modbusEntityEditingViewModel = new ModbusEntityEditingViewModel();
            modbusEntityEditingViewModel.SetEntity(modbusMemoryEntityViewModel.Clone() as ModbusMemoryEntityViewModel);

            MRNetworkingEditingViewxaml window = new MRNetworkingEditingViewxaml();
            window.DataContext = modbusEntityEditingViewModel;
            window.ShowDialog();

        }





        #endregion

        #region Implementation of IModbusMemoryViewModel

        public ICommand EditEntityCommand { get; }

        public ObservableCollection<ModbusMemoryEntityViewModel> ModbusMemoryEntityViewModels
        {
            get { return _modbusMemoryEntityViewModels; }
            set
            {
                _modbusMemoryEntityViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ModbusMemorySettingsViewModel ModbusMemorySettingsViewModel
        {
            get { return _modbusMemorySettingsViewModel; }
            set
            {
                _modbusMemorySettingsViewModel = value;
                _modbusConversionParametersViewModels = new List<ModbusConversionParametersViewModel>(32);
                _modbusMemorySettingsViewModel.ModbusMemorySettingsChanged += OnModbusMemorySettingsChanged;
                OnModbusMemorySettingsChanged(_modbusMemorySettingsViewModel.GetModbusMemorySettings());
                RaisePropertyChanged();
            }
        }

        private void OnModbusMemorySettingsChanged(ModbusMemorySettings modbusMemorySettings)
        {
            bool isQueriesStartedBefore = IsQueriesStarted;
            if (modbusMemorySettings == null) return;
            IsQueriesStarted = false;
            if (ModbusMemoryEntityViewModels.Count != modbusMemorySettings.NumberOfPoints)
            {
                ModbusMemoryEntityViewModels.Clear();
            }
            //всякие условия вокруг - оптимизация вывода, чтобы не перерисовывать все каждый раз
            for (int i = 0; i < modbusMemorySettings.NumberOfPoints; i++)
            {

                if (_modbusConversionParametersViewModels.Count < i + 1)
                {
                    _modbusConversionParametersViewModels.Add(new ModbusConversionParametersViewModel());
                }

                if (ModbusMemoryEntityViewModels.Count <= i)
                {
                    ModbusMemoryEntityViewModel modbusMemoryEntityViewModel =
                        new ModbusMemoryEntityViewModel(_modbusConversionParametersViewModels[i]);
                    ModbusMemoryEntityViewModels.Add(modbusMemoryEntityViewModel);
                }
                ModbusMemoryEntityViewModels[i].SetAddress(modbusMemorySettings.BaseAdress + i);
                ModbusMemoryEntityViewModels[i].SetError();
            }
            if (isQueriesStartedBefore)
            {
                IsQueriesStarted = true;
            }
            else
            {
                ModbusMemoryEntityViewModels.ForEach((model => model.SetError()));
            }
        }


        public bool IsQueriesStarted
        {
            get => _isQueriesStarted;
            set => SetProperty(ref _isQueriesStarted, value);
        }


        #endregion

        #region Implementation of IQuery


        public ushort[] Value { get; set; }

        public bool IsQueriesAllowed
        {
            get { return _isQueriesAllowed; }
            set { _isQueriesAllowed = value; }
        }

        public async Task Update()
        {
            ModbusMemorySettings modbusMemorySettings = _modbusMemorySettingsViewModel.GetModbusMemorySettings();

            try
            {
                IsQueriesStarted = true;
                var res = await RTUConnectionGlobal.GetDataByAddress(1,
                    (ushort)modbusMemorySettings.BaseAdress, (ushort)modbusMemorySettings.NumberOfPoints, false);
                if (res == null)
                {
                    return;
                }
                int index = 0;
                foreach (var modbusMemoryEntityViewModel in ModbusMemoryEntityViewModels)
                {
                    if (res.Length <= index)

                    {
                        return;
                    }

                    modbusMemoryEntityViewModel.SetUshortValue(res[index]);
                    index++;
                }
            }
            catch (Exception e)
            {
                foreach (var modbusMemoryEntityViewModel in ModbusMemoryEntityViewModels)
                {
                    modbusMemoryEntityViewModel.SetError();
                }
                return;
            }

        }

        public bool WriteContext()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}