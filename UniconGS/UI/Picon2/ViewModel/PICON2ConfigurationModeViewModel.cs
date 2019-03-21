using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.Utility;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace UniconGS.UI.Picon2.ViewModel
{
    /// <summary>
    ///     Вью-модель для режима конфигурации руно
    /// </summary>
    public class PICON2ConfigurationModeViewModel : BindableBase, INavigationAware
    {
        #region [Const]
        private const ushort ADDRESS = 0x3013;
        private const ushort LENGTH_WORD = 0x50;
        private const ushort RUNO_VERSION_ADDRESS = 0x400;
        private const int RUNO_VERSION_LENGHT = 10;

        private const string NO = "Нет";
        private const string SHEDULE1 = "Гр. 1";
        private const string SHEDULE1EK1 = "Гр. 1 + Ек. 1";
        private const string SHEDULE1EK2 = "Гр. 1 + Ек. 2";
        private const string SHEDULE1EK3 = "Гр. 1 + Ек. 3";
        private const string SHEDULE2 = "Гр. 2";
        private const string SHEDULE2EK1 = "Гр. 2 + Ек. 1";
        private const string SHEDULE2EK2 = "Гр. 2 + Ек. 2";
        private const string SHEDULE2EK3 = "Гр. 2 + Ек. 3";

        private const string SHEDULE3 = "Гр. 3";
        private const string SHEDULE3EK1 = "Гр. 3 + Ек. 1";
        private const string SHEDULE3EK2 = "Гр. 3 + Ек. 2";
        private const string SHEDULE3EK3 = "Гр. 3 + Ек. 3";
        private const string SHEDULE4 = "Гр. 4";







        private const string OUTPUTS_KU_RELAY_1 = "Модуль 1 Реле №1";
        private const string OUTPUTS_KU_RELAY_2 = "Модуль 1 Реле №2";
        private const string OUTPUTS_KU_RELAY_3 = "Модуль 1 Реле №3";
        private const string OUTPUTS_KU_RELAY_4 = "Модуль 1 Реле №4";
        private const string OUTPUTS_KU_RELAY_5 = "Модуль 1 Реле №5";
        private const string OUTPUTS_KU_RELAY_6 = "Модуль 1 Реле №6";
        private const string OUTPUTS_KU_RELAY_7 = "Модуль 1 Реле №7";
        private const string OUTPUTS_KU_RELAY_8 = "Модуль 1 Реле №8";
        private const string OUTPUTS_KU_RELAY_9 = "Модуль 2 Реле №1";
        private const string OUTPUTS_KU_RELAY_10 = "Модуль 2 Реле №2";
        private const string OUTPUTS_KU_RELAY_11 = "Модуль 2 Реле №3";
        private const string OUTPUTS_KU_RELAY_12 = "Модуль 2 Реле №4";
        private const string OUTPUTS_KU_RELAY_13 = "Модуль 2 Реле №5";
        private const string OUTPUTS_KU_RELAY_14 = "Модуль 2 Реле №6";
        private const string OUTPUTS_KU_RELAY_15 = "Модуль 2 Реле №7";
        private const string OUTPUTS_KU_RELAY_16 = "Модуль 2 Реле №8";


        private const string MANAGEMENT_KU_DISCRETE_1 = "Модуль 1 Д №1";
        private const string MANAGEMENT_KU_DISCRETE_2 = "Модуль 1 Д №2";
        private const string MANAGEMENT_KU_DISCRETE_3 = "Модуль 1 Д №3";
        private const string MANAGEMENT_KU_DISCRETE_4 = "Модуль 1 Д №4";
        private const string MANAGEMENT_KU_DISCRETE_5 = "Модуль 1 Д №5";
        private const string MANAGEMENT_KU_DISCRETE_6 = "Модуль 1 Д №6";
        private const string MANAGEMENT_KU_DISCRETE_7 = "Модуль 1 Д №7";
        private const string MANAGEMENT_KU_DISCRETE_8 = "Модуль 1 Д №8";

        private const string MANAGEMENT_KU_DISCRETE_9 = "Модуль 2 Д №1";
        private const string MANAGEMENT_KU_DISCRETE_10 = "Модуль 2 Д №2";
        private const string MANAGEMENT_KU_DISCRETE_11 = "Модуль 2 Д №3";
        private const string MANAGEMENT_KU_DISCRETE_12 = "Модуль 2 Д №4";
        private const string MANAGEMENT_KU_DISCRETE_13 = "Модуль 2 Д №5";
        private const string MANAGEMENT_KU_DISCRETE_14 = "Модуль 2 Д №6";
        private const string MANAGEMENT_KU_DISCRETE_15 = "Модуль 2 Д №7";
        private const string MANAGEMENT_KU_DISCRETE_16 = "Модуль 2 Д №8";

        private const string MANAGEMENT_KU_DISCRETE_17 = "Модуль 3 Д №1";
        private const string MANAGEMENT_KU_DISCRETE_18 = "Модуль 3 Д №2";
        private const string MANAGEMENT_KU_DISCRETE_19 = "Модуль 3 Д №3";
        private const string MANAGEMENT_KU_DISCRETE_20 = "Модуль 3 Д №4";
        private const string MANAGEMENT_KU_DISCRETE_21 = "Модуль 3 Д №5";
        private const string MANAGEMENT_KU_DISCRETE_22 = "Модуль 3 Д №6";
        private const string MANAGEMENT_KU_DISCRETE_23 = "Модуль 3 Д №7";
        private const string MANAGEMENT_KU_DISCRETE_24 = "Модуль 3 Д №8";

        private const string MANAGEMENT_KU_DISCRETE_25 = "Модуль 4 Д №1";
        private const string MANAGEMENT_KU_DISCRETE_26 = "Модуль 4 Д №2";
        private const string MANAGEMENT_KU_DISCRETE_27 = "Модуль 4 Д №3";
        private const string MANAGEMENT_KU_DISCRETE_28 = "Модуль 4 Д №4";
        private const string MANAGEMENT_KU_DISCRETE_29 = "Модуль 4 Д №5";
        private const string MANAGEMENT_KU_DISCRETE_30 = "Модуль 4 Д №6";
        private const string MANAGEMENT_KU_DISCRETE_31 = "Модуль 4 Д №7";
        private const string MANAGEMENT_KU_DISCRETE_32 = "Модуль 4 Д №8";

        private const string MANAGEMENT_KU_DISCRETE_33 = "Модуль 5 Д №1";
        private const string MANAGEMENT_KU_DISCRETE_34 = "Модуль 5 Д №2";
        private const string MANAGEMENT_KU_DISCRETE_35 = "Модуль 5 Д №3";
        private const string MANAGEMENT_KU_DISCRETE_36 = "Модуль 5 Д №4";
        private const string MANAGEMENT_KU_DISCRETE_37 = "Модуль 5 Д №5";
        private const string MANAGEMENT_KU_DISCRETE_38 = "Модуль 5 Д №6";
        private const string MANAGEMENT_KU_DISCRETE_39 = "Модуль 5 Д №7";
        private const string MANAGEMENT_KU_DISCRETE_40 = "Модуль 5 Д №8";

        private const string MANAGEMENT_KU_DISCRETE_41 = "Модуль 6 Д №1";
        private const string MANAGEMENT_KU_DISCRETE_42 = "Модуль 6 Д №2";
        private const string MANAGEMENT_KU_DISCRETE_43 = "Модуль 6 Д №3";
        private const string MANAGEMENT_KU_DISCRETE_44 = "Модуль 6 Д №4";
        private const string MANAGEMENT_KU_DISCRETE_45 = "Модуль 6 Д №5";
        private const string MANAGEMENT_KU_DISCRETE_46 = "Модуль 6 Д №6";
        private const string MANAGEMENT_KU_DISCRETE_47 = "Модуль 6 Д №7";
        private const string MANAGEMENT_KU_DISCRETE_48 = "Модуль 6 Д №8";

        private const string MANAGEMENT_KU_DISCRETE_49 = "Модуль 7 Д №1";
        private const string MANAGEMENT_KU_DISCRETE_50 = "Модуль 7 Д №2";
        private const string MANAGEMENT_KU_DISCRETE_51 = "Модуль 7 Д №3";
        private const string MANAGEMENT_KU_DISCRETE_52 = "Модуль 7 Д №4";
        private const string MANAGEMENT_KU_DISCRETE_53 = "Модуль 7 Д №5";
        private const string MANAGEMENT_KU_DISCRETE_54 = "Модуль 7 Д №6";
        private const string MANAGEMENT_KU_DISCRETE_55 = "Модуль 7 Д №7";
        private const string MANAGEMENT_KU_DISCRETE_56 = "Модуль 7 Д №8";

        private const string MANAGEMENT_KU_DISCRETE_57 = "Модуль 8 Д №1";
        private const string MANAGEMENT_KU_DISCRETE_58 = "Модуль 8 Д №2";
        private const string MANAGEMENT_KU_DISCRETE_59 = "Модуль 8 Д №3";
        private const string MANAGEMENT_KU_DISCRETE_60 = "Модуль 8 Д №4";
        private const string MANAGEMENT_KU_DISCRETE_61 = "Модуль 8 Д №5";
        private const string MANAGEMENT_KU_DISCRETE_62 = "Модуль 8 Д №6";
        private const string MANAGEMENT_KU_DISCRETE_63 = "Модуль 8 Д №7";
        private const string MANAGEMENT_KU_DISCRETE_64 = "Модуль 8 Д №8";



        #endregion

        #region [Private Fields]

        private string _deviceName;
        private string _versionName = String.Empty;
        private ICommand _sendConfiguration;
        private ICommand _getConfiguration;
        private ICommand _getConfigurationFromFileCommand;
        private ICommand _saveConfigurationInFileCommand;
        private ICommand _backToSchemeCommand;
        private ObservableCollection<PICON2ConfigCheckBoxControlViewModel> _faultCanals;
        private PICON2ConfigCheckBoxControlViewModel _faultPower;
        private PICON2ConfigCheckBoxControlViewModel _faultManagemet;
        private PICON2ConfigCheckBoxControlViewModel _faultSecurity;
        private PICON2ConfigCheckBoxControlViewModel _faultMask;
        private int _timeToStart;
        private ObservableCollection<string> _dataKuSelected;
        private ObservableCollection<string> _outputsKuSelected;
        private ObservableCollection<string> _outputsKuSelectedInv;
        private ObservableCollection<string> _managementKuSelected;
        private ObservableCollection<string> _dataKuCollection;
        private ObservableCollection<string> _outputsKuCollection;
        private ObservableCollection<string> _managementKuCollection;
        private ObservableCollection<string> _tempManagementKuCollection;
        private ObservableCollection<string> _tempOutputKuSelected;
        private ObservableCollection<string> _tempOutputKuSelectedInv;


        private bool[] _diskretArrFromManagementKU;
        private bool _isReadAndInitializeNow = false;

        // Словарь где имени графика соотносится его hex значение в устройстве
        private readonly Dictionary<string, ushort> _dataNameValueDictionary = new Dictionary<string, ushort>
        {
            {NO, 0},
            {SHEDULE1, 1},
            {SHEDULE1EK1, 2},
            {SHEDULE1EK2, 3},
            {SHEDULE1EK3, 4},
            {SHEDULE2, 5},
            {SHEDULE2EK1, 6},
            {SHEDULE2EK2, 7},
            {SHEDULE2EK3, 8},
            {SHEDULE3, 9},
            {SHEDULE3EK1, 10},
            {SHEDULE3EK2, 11},
            {SHEDULE3EK3, 12},
            {SHEDULE4, 13},
        };

        // Словарь где имени реле соотносится его hex значение в устройстве
        private readonly Dictionary<string, ushort> _outputsNameValueDictionary = new Dictionary<string, ushort>
        {
            {NO, 0},
            {OUTPUTS_KU_RELAY_1, 1},
            {OUTPUTS_KU_RELAY_2, 2},
            {OUTPUTS_KU_RELAY_3, 3},
            {OUTPUTS_KU_RELAY_4, 4},
            {OUTPUTS_KU_RELAY_5, 5},
            {OUTPUTS_KU_RELAY_6, 6},
            {OUTPUTS_KU_RELAY_7, 7},
            {OUTPUTS_KU_RELAY_8, 8},
            {OUTPUTS_KU_RELAY_9, 9},
            {OUTPUTS_KU_RELAY_10, 10},
            {OUTPUTS_KU_RELAY_11, 11},
            {OUTPUTS_KU_RELAY_12, 12},
            {OUTPUTS_KU_RELAY_13, 13},
            {OUTPUTS_KU_RELAY_14, 14},
            {OUTPUTS_KU_RELAY_15, 15},
            {OUTPUTS_KU_RELAY_16, 16},

        };

        // Словарь где имени дискрета управления соотносится его hex значение в устройстве
        private readonly Dictionary<string, ushort> _managementNameValueDictionary = new Dictionary<string, ushort>
        {
            {NO, 0},
            {MANAGEMENT_KU_DISCRETE_1, 1},
            {MANAGEMENT_KU_DISCRETE_2, 2},
            {MANAGEMENT_KU_DISCRETE_3, 3},
            {MANAGEMENT_KU_DISCRETE_4, 4},
            {MANAGEMENT_KU_DISCRETE_5, 5},
            {MANAGEMENT_KU_DISCRETE_6, 6},
            {MANAGEMENT_KU_DISCRETE_7, 7},
            {MANAGEMENT_KU_DISCRETE_8, 8},
            {MANAGEMENT_KU_DISCRETE_9, 9},
            {MANAGEMENT_KU_DISCRETE_10, 10},
            {MANAGEMENT_KU_DISCRETE_11, 11},


            {MANAGEMENT_KU_DISCRETE_12, 12},
            {MANAGEMENT_KU_DISCRETE_13, 13},
            {MANAGEMENT_KU_DISCRETE_14, 14},
            {MANAGEMENT_KU_DISCRETE_15, 15},
            {MANAGEMENT_KU_DISCRETE_16, 16},
            {MANAGEMENT_KU_DISCRETE_17, 17},
            {MANAGEMENT_KU_DISCRETE_18, 18},
            {MANAGEMENT_KU_DISCRETE_19, 19},
            {MANAGEMENT_KU_DISCRETE_20, 20},
            {MANAGEMENT_KU_DISCRETE_21, 21},
            {MANAGEMENT_KU_DISCRETE_22, 22},

            {MANAGEMENT_KU_DISCRETE_23, 23},
            {MANAGEMENT_KU_DISCRETE_24, 24},
            {MANAGEMENT_KU_DISCRETE_25, 25},
            {MANAGEMENT_KU_DISCRETE_26, 26},
            {MANAGEMENT_KU_DISCRETE_27, 27},
            {MANAGEMENT_KU_DISCRETE_28, 28},
            {MANAGEMENT_KU_DISCRETE_29, 29},
            {MANAGEMENT_KU_DISCRETE_30, 30},
            {MANAGEMENT_KU_DISCRETE_31, 31},
            {MANAGEMENT_KU_DISCRETE_32, 32},
            {MANAGEMENT_KU_DISCRETE_33, 33},

            {MANAGEMENT_KU_DISCRETE_34, 34},
            {MANAGEMENT_KU_DISCRETE_35, 35},
            {MANAGEMENT_KU_DISCRETE_36, 36},
            {MANAGEMENT_KU_DISCRETE_37, 37},
            {MANAGEMENT_KU_DISCRETE_38, 38},
            {MANAGEMENT_KU_DISCRETE_39, 39},
            {MANAGEMENT_KU_DISCRETE_40, 40},
            {MANAGEMENT_KU_DISCRETE_41, 41},
            {MANAGEMENT_KU_DISCRETE_42, 42},
            {MANAGEMENT_KU_DISCRETE_43, 43},

            {MANAGEMENT_KU_DISCRETE_44, 44},
            {MANAGEMENT_KU_DISCRETE_45, 45},
            {MANAGEMENT_KU_DISCRETE_46, 46},
            {MANAGEMENT_KU_DISCRETE_47, 47},
            {MANAGEMENT_KU_DISCRETE_48, 48},
            {MANAGEMENT_KU_DISCRETE_49, 49},
            {MANAGEMENT_KU_DISCRETE_50, 50},
            {MANAGEMENT_KU_DISCRETE_51, 51},
            {MANAGEMENT_KU_DISCRETE_52, 52},
            {MANAGEMENT_KU_DISCRETE_53, 53},
            {MANAGEMENT_KU_DISCRETE_54, 54},


            {MANAGEMENT_KU_DISCRETE_55, 55},
            {MANAGEMENT_KU_DISCRETE_56, 56},
            {MANAGEMENT_KU_DISCRETE_57, 57},
            {MANAGEMENT_KU_DISCRETE_58, 58},
            {MANAGEMENT_KU_DISCRETE_59, 59},
            {MANAGEMENT_KU_DISCRETE_60, 60},
            {MANAGEMENT_KU_DISCRETE_61, 61},
            {MANAGEMENT_KU_DISCRETE_62, 62},
            {MANAGEMENT_KU_DISCRETE_63, 63},
            {MANAGEMENT_KU_DISCRETE_64, 64},

        };

        //// Словарь где имени данного соотносится его hex значение в устройстве
        //private readonly Dictionary<string, ushort> _nameValueDictionar = new Dictionary<string, ushort>
        //{
        //    {NO, 0x0000},
        //    {DATA_KU_LIGHTING, 0x0001}, {DATA_KU_BACKLIGHT, 0x0002}, {DATA_KU_ILLUMINATION, 0x0003}, {DATA_KU_ENERGY_SAVING, 0x0004},
        //    {DATA_KU_HEATING, 0x0005}, {DATA_KU_ECONOMY_LIGHTING, 0x0006}, {DATA_KU_ECONOMY_BACKLIGHT, 0x0007}, {DATA_KU_ECONOMY_ILLUMINATION, 0x0008},
        //    {OUTPUTS_KU_RELAY_1, 0x01}, {OUTPUTS_KU_RELAY_2, 0x02}, {OUTPUTS_KU_RELAY_3, 0x03}, 
        //    {MANAGEMENT_KU_DISCRETE_1, 0x01}, {MANAGEMENT_KU_DISCRETE_2, 0x02}, {MANAGEMENT_KU_DISCRETE_3, 0x03}, {MANAGEMENT_KU_DISCRETE_4, 0x04}, 
        //    {MANAGEMENT_KU_DISCRETE_5, 0x05}, {MANAGEMENT_KU_DISCRETE_6, 0x06}, {MANAGEMENT_KU_DISCRETE_7, 0x07}, {MANAGEMENT_KU_DISCRETE_8, 0x08}, 
        //    {MANAGEMENT_KU_DISCRETE_9, 0x09},  {MANAGEMENT_KU_DISCRETE_10, 0xA}, {MANAGEMENT_KU_DISCRETE_11, 0xB}
        //};

        // Маска реле неисправности канала
        private readonly ushort[] _maskRelayFaultCanal = new ushort[11]
        {
            0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0080, 0x0100, 0x0200, 0x0400
        };


        private Dictionary<string, object> _navigationContext = new Dictionary<string, object>();
        private List<DateEnterFieldViewModel> _sheduleDates;
        private int _controllersNumber;

        private bool _isAutonomus;

        #endregion [Private Fields]

        #region [Ctor's]

        /// <summary>
        ///     Конструктор класса <see cref="PICON2ConfigurationModeViewModel"/>
        /// </summary>
        public PICON2ConfigurationModeViewModel()
        {
            Refresh();
            OnNavigatedTo(null);
        }


        private void Refresh()
        {
            var faultList = new List<PICON2ConfigCheckBoxControlViewModel>();
            for (int i = 0; i < 8; i++)
            {
                faultList.Add(new PICON2ConfigCheckBoxControlViewModel());
            }
            this.FaultCanals = new ObservableCollection<PICON2ConfigCheckBoxControlViewModel>(faultList);
            this.FaultMask = new PICON2ConfigCheckBoxControlViewModel();
            this.FaultManagement = new PICON2ConfigCheckBoxControlViewModel();
            this.FaultPower = new PICON2ConfigCheckBoxControlViewModel();
            this.FaultSecurity = new PICON2ConfigCheckBoxControlViewModel();
            this.DataKuSelected = new ObservableCollection<string>
            {
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO
            };
            this.OutputsKuSelected = new ObservableCollection<string>
            {
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO
            };
            this.OutputsKuSelectedInv = new ObservableCollection<string>()
            {
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO
            };
            this.ManagementKuSelected = new ObservableCollection<string>
            {
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO
            };
            _tempManagementKuCollection = new ObservableCollection<string>()
            {
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO
            };

            this._tempOutputKuSelected = new ObservableCollection<string>
            {
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO
            };
            this._tempOutputKuSelectedInv = new ObservableCollection<string>
            {
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO,
                NO
            };
            this.TimeToStart = 100;
            this._diskretArrFromManagementKU = new bool[64];

            SheduleDates = new List<DateEnterFieldViewModel>();
            for (int i = 0; i < 8; i++)
            {
                SheduleDates.Add(new DateEnterFieldViewModel());
            }
        }

        #endregion

        #region [Properties]

        public List<DateEnterFieldViewModel> SheduleDates
        {
            get { return _sheduleDates; }
            set
            {
                _sheduleDates = value;
                OnPropertyChanged(nameof(SheduleDates));
            }
        }


        /// <summary>
        ///     Описывает версию устройства
        /// </summary>
        public string VersionDescription
        {
            get { return this._versionName; }
            set
            {
                if (this._versionName != null && this._versionName.Equals(value)) return;
                this._versionName = value;
                OnPropertyChanged("RunoVersionDescription");
            }
        }

        /// <summary>
        /// Флаг, true - если происходит считывание
        /// и передача конфигурации на вьюшку в данный момент 
        /// </summary>
        public Boolean IsInitializeNow
        {
            get { return this._isReadAndInitializeNow; }
            set { this._isReadAndInitializeNow = value; }
        }

        /// <summary>
        ///  Представляет временную коллекцию для комбобокс (Контроль управления)
        /// </summary>
        public ObservableCollection<string> TempManagmentCollection
        {
            get { return this._tempManagementKuCollection; }
            set { this._tempManagementKuCollection = value; }
        }

        /// <summary>
        ///     Представляет временную коллекцию для комбобокс (Выходы КУ)
        /// </summary>
        public ObservableCollection<string> TempOutputKuSelected
        {
            get { return this._tempOutputKuSelected; }
            set { this._tempOutputKuSelected = value; }
        }


        /// <summary>
        ///     Представляет временную коллекцию для комбобокс (Выходы КУ)
        /// </summary>
        public ObservableCollection<string> TempOutputKuSelectedInv
        {
            get { return this._tempOutputKuSelectedInv; }
            set { this._tempOutputKuSelectedInv = value; }
        }


        /// <summary>
        ///     Представляет коллекцию для неисправностей каналов(Чекбоксы)
        /// index + 1 == номер канала
        /// </summary>
        public ObservableCollection<PICON2ConfigCheckBoxControlViewModel> FaultCanals
        {
            get { return this._faultCanals; }
            set
            {
                if (this._faultCanals != null && this._faultCanals.Equals(value)) return;
                this._faultCanals = value;
                OnPropertyChanged("FaultCanals");
            }
        }

        /// <summary>
        ///     Представляет вью-модель для представления Маски неисправностей
        /// </summary>
        public PICON2ConfigCheckBoxControlViewModel FaultMask
        {
            get { return this._faultMask; }
            set
            {
                if (this._faultMask != null && this._faultMask.Equals(value)) return;
                this._faultMask = value;
                OnPropertyChanged("FaultMask");
            }
        }

        /// <summary>
        ///     Представляет вью-модель для представления неисправностей Режима управления
        /// </summary>
        public PICON2ConfigCheckBoxControlViewModel FaultManagement
        {
            get { return this._faultManagemet; }
            set
            {
                if (this._faultManagemet != null && this._faultManagemet.Equals(value)) return;
                this._faultManagemet = value;
                OnPropertyChanged("FaultManagement");
            }
        }

        /// <summary>
        ///     Представляет вью-модель для представления неисправностей Питания
        /// </summary>
        public PICON2ConfigCheckBoxControlViewModel FaultPower
        {
            get { return this._faultPower; }
            set
            {
                if (this._faultPower != null && this._faultPower.Equals(value)) return;
                this._faultPower = value;
                OnPropertyChanged("FaultPower");
            }
        }

        /// <summary>
        ///     Представляет вью-модель для представления неисправностей Охраны
        /// </summary>
        public PICON2ConfigCheckBoxControlViewModel FaultSecurity
        {
            get { return this._faultSecurity; }
            set
            {
                if (this._faultSecurity != null && this._faultSecurity.Equals(value)) return;
                this._faultSecurity = value;
                OnPropertyChanged("FaultSecurity");
            }
        }

        /// <summary>
        ///     Представляет результат выбранного в Combobox режима Контроля управления
        ///     для каждого канала
        ///     номер канала == index + 1
        /// </summary>
        public ObservableCollection<string> ManagementKuSelected
        {
            get { return this._managementKuSelected; }
            set
            {
                if (this._managementKuSelected != null && this._managementKuSelected.Equals(value)) return;
                this._managementKuSelected = value;
                OnPropertyChanged("ManagementKuSelected");
            }
        }

        /// <summary>
        ///     Представляет результат выбранного в Combobox режима Данные КУ
        ///     для каждого канала
        ///     номер канала == index + 1
        /// </summary>
        public ObservableCollection<string> DataKuSelected
        {
            get { return this._dataKuSelected; }
            set
            {
                if (this._dataKuSelected != null && this._dataKuSelected.Equals(value)) return;
                this._dataKuSelected = value;
                OnPropertyChanged("DataKuSelected");
            }
        }

        /// <summary>
        ///     Представляет результат выбранного в Combobox режима Выходы КУ
        ///     для каждого канала
        ///     номер канала == index + 1
        /// </summary>
        public ObservableCollection<string> OutputsKuSelected
        {
            get { return this._outputsKuSelected; }
            set
            {
                if (this._outputsKuSelected != null && this._outputsKuSelected.Equals(value)) return;
                this._outputsKuSelected = value;
                OnPropertyChanged("OutputsKuSelected");
            }
        }

        /// <summary>
        ///     Представляет результат выбранного в Combobox режима Выходы КУ на отключение
        ///     для каждого канала
        ///     номер канала == index + 1
        /// </summary>
        public ObservableCollection<string> OutputsKuSelectedInv
        {
            get { return this._outputsKuSelectedInv; }
            set
            {
                if (this._outputsKuSelectedInv != null && this._outputsKuSelectedInv.Equals(value)) return;
                this._outputsKuSelectedInv = value;
                OnPropertyChanged("OutputsKuSelectedInv");
            }
        }

        /// <summary>
        ///     Вернет коллекцию со списком значений для режима Контроль управления
        /// </summary>
        public ObservableCollection<string> ManagementKuCollection
        {
            get
            {
                _managementKuCollection = new ObservableCollection<string>(_managementNameValueDictionary.Keys);
                return _managementKuCollection;
            }
        }

        /// <summary>
        ///     Вернет коллекцию со списком значений для режима Данные КУ
        /// </summary>
        public ObservableCollection<string> DataKuCollection
        {
            get
            {
                return this._dataKuCollection ??
                       (this._dataKuCollection = new ObservableCollection<string>()
                       {
                           NO,
                           SHEDULE1,
                           SHEDULE1EK1,
                           SHEDULE1EK2,
                           SHEDULE1EK3,
                           SHEDULE2,
                           SHEDULE2EK1,
                           SHEDULE2EK2,
                           SHEDULE2EK3,
                           SHEDULE3,
                           SHEDULE3EK1,
                           SHEDULE3EK2,
                           SHEDULE3EK3,
                           SHEDULE4,
                       });
            }
        }

        /// <summary>
        ///     Вернет коллекцию со списком значений для режима Выходы КУ
        /// </summary>
        public ObservableCollection<string> OutputsKuCollection
        {
            get
            {
                _outputsKuCollection = new ObservableCollection<string>(_outputsNameValueDictionary.Keys);
                return _outputsKuCollection;
            }
        }

        /// <summary>
        ///     Включение автоматики (в секундах)
        /// </summary>
        public int TimeToStart
        {
            get { return this._timeToStart; }
            set
            {
                if (this._timeToStart.Equals(value)) return;
                this._timeToStart = value;
                OnPropertyChanged("TimeToStart");
            }
        }
        /// <summary>
        /// Число подкл. контроллеров
        /// </summary>
        public int ControllersNumber
        {
            get { return _controllersNumber; }
            set
            {
                _controllersNumber = value;
                OnPropertyChanged("ControllersNumber");
            }
        }


        public bool IsAutonomus
        {
            get { return _isAutonomus; }
            set
            {
                _isAutonomus = value;
                RaisePropertyChanged();
            }
        }
        #endregion [Properties]


        #region [IRunoConfigurationModeViewModel]

        /// <summary>
        ///     Свойство представляющие имя устройства, для которого производится конфигурация
        /// </summary>
        public string DeviceName
        {
            get { return this._deviceName; }
            set
            {
                if (this._deviceName != null && this._deviceName.Equals(value)) return;
                this._deviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }

        /// <summary>
        ///      Представляет команду отправки конфигурационных данных на устройство
        /// </summary>
        public ICommand SendConfiguration
        {
            get
            {
                return this._sendConfiguration ?? (this._sendConfiguration = new DelegateCommand(OnSendConfiguration));
            }
        }

        /// <summary>
        ///     Представляет команду считывания конфигурационных данных с устройства
        /// </summary>
        public ICommand GetConfiguration
        {
            get
            {
                return this._getConfiguration ?? (this._getConfiguration =
                           new DelegateCommand(InitializingOnNavigateTo));
            }
        }

        /// <summary>
        ///     Загружает конфигурацию из файла
        /// </summary>
        public ICommand GetConfigurationFromFileCommand
        {
            get
            {
                return this._getConfigurationFromFileCommand ?? (this._getConfigurationFromFileCommand =
                           new DelegateCommand(OnGetConfigurationFromFileCommand));
            }
        }

        /// <summary>
        ///     Сохраняет конфигурацию в файл
        /// </summary>
        public ICommand SaveConfigurationInFileCommand
        {
            get
            {
                return this._saveConfigurationInFileCommand ??
                       (this._saveConfigurationInFileCommand = new DelegateCommand(
                           OnSaveConfigurationInFileCommand));
            }
        }



        #endregion [IRunoConfigurationModeViewModel]

        #region [INavigationAware]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns></returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        ///     При переходе с вьюхи
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this._deviceName = null;
            this._sendConfiguration = null;
            this.FaultManagement.PropertyChanged -= FaultOnPropertyChanged;
            this.FaultPower.PropertyChanged -= FaultOnPropertyChanged;
            this.FaultSecurity.PropertyChanged -= FaultOnPropertyChanged;
            this.DataKuSelected.CollectionChanged -= ManagementKuSelectedOnCollectionChanged;
            this.OutputsKuSelected.CollectionChanged -= OutputsKuSelectedOnCollectionChanged;
            this.OutputsKuSelectedInv.CollectionChanged -= OutputsKuSelectedOnCollectionChangedInv;

            VersionDescription = String.Empty;
            foreach (var faultCanal in this.FaultCanals)
            {
                faultCanal.PropertyChanged -= FaultOnPropertyChanged;
            }
        }

        /// <summary>
        ///     При переходе на вьюху
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
            _navigationContext.Clear();


            this.FaultManagement.PropertyChanged += FaultOnPropertyChanged;
            this.FaultPower.PropertyChanged += FaultOnPropertyChanged;
            this.FaultSecurity.PropertyChanged += FaultOnPropertyChanged;
            foreach (var faultCanal in this.FaultCanals)
            {
                faultCanal.PropertyChanged += FaultOnPropertyChanged;
            }

            // this.InitializingOnNavigateTo();
            this.ManagementKuSelected.CollectionChanged += ManagementKuSelectedOnCollectionChanged;
            this.OutputsKuSelected.CollectionChanged += OutputsKuSelectedOnCollectionChanged;
            this.OutputsKuSelectedInv.CollectionChanged += OutputsKuSelectedOnCollectionChangedInv;



        }



        #endregion [INavigationAware]

        #region [Help Members]

        private const string DECLARATION_VERSION = "1.0";
        private const string DECLARATION_ENCODING = "utf-8";

        private void OnSaveConfigurationInFileCommand()
        {
            var fileDialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "Configuration files (*.lc)|*.lc",
                Title = "Сохраните файл"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var xDoc = new XDocument(new XDeclaration(DECLARATION_VERSION, DECLARATION_ENCODING, ""));
                var root = new XElement("ChannelManagment");
                root.Add(new XElement("Device", "PICON2"));
                root.Add(new XElement("ControllersNumber", this.ControllersNumber));

                root.Add(new XElement("AutomationTime", this.TimeToStart));
                for (int i = 0; i < this.DataKuSelected.Count; i++)
                {
                    root.Add(new XElement("Channels", new object[]
                    {
                        new XElement("IsTurnOn", false),
                        new XElement("GraphicValue", this.DataKuCollection.IndexOf(this.DataKuSelected[i])),
                        new XElement("DiscretValue", this.ManagementKuCollection.IndexOf(this.ManagementKuSelected[i])),
                        new XElement("ReleValue", this.OutputsKuCollection.IndexOf(this.OutputsKuSelected[i])),
                        new XElement("ReleValueInv",this.OutputsKuCollection.IndexOf(this.OutputsKuSelectedInv[i]))
                    }));
                }

                var checkBoxCount = 64;

                for (int i = 0; i < this.FaultCanals.Count; i++)
                {
                    var channelMasksElement = new XElement("ChannelMasks");
                    for (int j = 0; j < checkBoxCount; j++)
                    {
                        channelMasksElement.Add(new XElement("Value", this.FaultCanals[i][j]));
                    }
                    root.Add(channelMasksElement);
                }
                var shedulesXElement = new XElement("Shedules");

                foreach (var shedule in SheduleDates)
                {
                    var sheduleItemElement = new XElement("SheduleItem");
                    sheduleItemElement.Add(new XElement("DatePart1", shedule.DatePart1));
                    sheduleItemElement.Add(new XElement("DatePart2", shedule.DatePart2));
                    shedulesXElement.Add(sheduleItemElement);
                }
                root.Add(shedulesXElement);




                var securityMaskElement = new XElement("SecurityMask");
                for (int i = 0; i < checkBoxCount; i++)
                {
                    securityMaskElement.Add(new XElement("Value", this.FaultSecurity[i]));
                }
                root.Add(securityMaskElement);

                var managmentMaskElement = new XElement("ManagmentMask");
                for (int i = 0; i < checkBoxCount; i++)
                {
                    managmentMaskElement.Add(new XElement("Value", this.FaultManagement[i]));
                }
                root.Add(managmentMaskElement);

                var powerMaskElement = new XElement("PowerMask");
                for (int i = 0; i < checkBoxCount; i++)
                {
                    powerMaskElement.Add(new XElement("Value", this.FaultPower[i]));
                }
                root.Add(powerMaskElement);

                var errorMaskElement = new XElement("ErrorMask");
                for (int i = 0; i < checkBoxCount; i++)
                {
                    errorMaskElement.Add(new XElement("Value", this.FaultMask.CheckBoxResult[i]));
                }
                root.Add(errorMaskElement);

                xDoc.Add(root);
                xDoc.Save(fileDialog.FileName);
            }
            catch (Exception exception)
            {
                this.InteractWithError(exception);
            }
        }

        private void OnGetConfigurationFromFileCommand()
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Configuration files (*.lc)|*.lc",
                Title = "Выберите файл"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                for (int i = 0; i < 8; i++)
                {
                    this.DataKuSelected[i] = this.DataKuCollection[0];
                    this.OutputsKuSelected[i] = this.OutputsKuCollection[0];
                    this.OutputsKuSelectedInv[i] = this.OutputsKuCollection[0];
                    this.ManagementKuSelected[i] = this.ManagementKuCollection[0];
                    for (int j = 0; j < 64; j++)
                    {
                        this.FaultCanals[i][j] = false;
                    }
                }
                for (int j = 0; j < 64; j++)
                {
                    this.FaultSecurity[j] = false;
                    this.FaultPower[j] = false;
                    this.FaultManagement[j] = false;
                }
                var doc = XDocument.Load(fileDialog.FileName);

                this.IsInitializeNow = true;

                int channelsCounter = 0, channelMasksCounter = 0;
                foreach (XElement xElement in doc.Root.Elements())
                {
                    if (xElement.Name.ToString().Equals("Device"))
                    {
                        if (xElement.Value.ToString(CultureInfo.InvariantCulture) != "PICON2")
                        {
                            this.InteractWithError(
                                new Exception("Файл от устройства: " +
                                              xElement.Value.ToString(CultureInfo.InvariantCulture) +
                                              ". Требуется файл, соответствующий устройству: PICON2"));
                            this.IsInitializeNow = false;
                            return;
                        }
                    }


                    if (xElement.Name.ToString().Equals("AutomationTime"))
                    {
                        this.TimeToStart = Int32.Parse(xElement.Value.ToString(CultureInfo.InvariantCulture));
                    }
                    if (xElement.Name.ToString().Equals("ControllersNumber"))
                    {
                        this.ControllersNumber = Int32.Parse(xElement.Value.ToString(CultureInfo.InvariantCulture));
                    }
                    if (xElement.Name.ToString().Equals("Shedules"))
                    {
                        int i = 0;
                        foreach (var element in xElement.Elements())
                        {
                            if (element.Name.ToString() == "SheduleItem")
                            {
                                foreach (var dateElement in element.Elements())
                                {
                                    if (dateElement.Name.ToString() == "DatePart1")
                                    {
                                        SheduleDates[i].DatePart1 =
                                            dateElement.Value.ToString(CultureInfo.InvariantCulture);
                                    }
                                    if (dateElement.Name.ToString() == "DatePart2")
                                    {
                                        SheduleDates[i].DatePart2 =
                                            dateElement.Value.ToString(CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            i++;
                        }
                    }



                    if (xElement.Name.ToString().Equals("Channels"))
                    {
                        foreach (XElement element in xElement.Elements())
                        {
                            if (element.Name.ToString().Equals("GraphicValue"))
                            {
                                this.DataKuSelected[channelsCounter] =
                                    this.DataKuCollection[
                                        Int32.Parse(element.Value.ToString(CultureInfo.InvariantCulture))];
                            }
                            if (element.Name.ToString().Equals("ReleValue"))
                            {
                                this.OutputsKuSelected[channelsCounter] =
                                    this.OutputsKuCollection[
                                        Int32.Parse(element.Value.ToString(CultureInfo.InvariantCulture))];
                            }
                            if (element.Name.ToString().Equals("ReleValueInv"))
                            {
                                this.OutputsKuSelectedInv[channelsCounter] =
                                    this.OutputsKuCollection[
                                        Int32.Parse(element.Value.ToString(CultureInfo.InvariantCulture))];
                            }
                            if (element.Name.ToString().Equals("DiscretValue"))
                            {
                                this.ManagementKuSelected[channelsCounter] =
                                    this.ManagementKuCollection[
                                        Int32.Parse(element.Value.ToString(CultureInfo.InvariantCulture))];
                            }
                        }
                        channelsCounter++;
                    }
                    if (xElement.Name.ToString().Equals("ChannelMasks"))
                    {
                        int i = 0;
                        foreach (var element in xElement.Elements())
                        {
                            this.FaultCanals[channelMasksCounter][i] =
                                Boolean.Parse(element.Value.ToString(CultureInfo.InvariantCulture));
                            i++;
                            if (i > 63) break;
                        }

                        channelMasksCounter++;
                    }
                    if (xElement.Name.ToString().Equals("SecurityMask"))
                    {
                        int i = 0;
                        foreach (var element in xElement.Elements())
                        {
                            this.FaultSecurity[i] = Boolean.Parse(element.Value.ToString(CultureInfo.InvariantCulture));
                            i++;
                            if (i > 63) break;
                        }
                    }
                    if (xElement.Name.ToString().Equals("ManagmentMask"))
                    {
                        int i = 0;
                        foreach (var element in xElement.Elements())
                        {
                            this.FaultManagement[i] =
                                Boolean.Parse(element.Value.ToString(CultureInfo.InvariantCulture));
                            i++;
                            if (i > 63) break;
                        }
                    }
                    if (xElement.Name.ToString().Equals("PowerMask"))
                    {
                        int i = 0;
                        foreach (var element in xElement.Elements())
                        {
                            this.FaultPower[i] = Boolean.Parse(element.Value.ToString(CultureInfo.InvariantCulture));
                            i++;
                            if (i > 63) break;
                        }
                    }
                }
            }
            catch
            {
                this.InteractWithError(new Exception("Файл не может быть прочитан"));
            }
            finally
            {
                this.IsInitializeNow = false;
            }

        }

        private void OutputsKuSelectedOnCollectionChangedInv(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var newOutputKuSelected = OutputsKuSelected.Where(output => output != NO).ToList();
            newOutputKuSelected.AddRange(OutputsKuSelectedInv.Where(output => output != NO).ToList());
            var allOutputCount = newOutputKuSelected.Count();
            var matchCount = newOutputKuSelected.Distinct().Count();
            //Если значения не равны, значит появилось совпадение
            if (allOutputCount != matchCount)
            {
                //после выхода из обработчика, попадаем в другой(в codebehind) который отменит выбранный дискрет
                return;
            }
            else
            {
                for (int i = 0; i < TempOutputKuSelectedInv.Count; i++)
                {
                    this.TempOutputKuSelectedInv[i] = OutputsKuSelectedInv[i];
                }
            }
        }
        private void OutputsKuSelectedOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var newOutputKuSelected = OutputsKuSelected.Where(output => output != NO).ToList();
            newOutputKuSelected.AddRange(OutputsKuSelectedInv.Where(output => output != NO).ToList());
            var allOutputCount = newOutputKuSelected.Count();
            var matchCount = newOutputKuSelected.Distinct().Count();
            //Если значения не равны, значит появилось совпадение
            if (allOutputCount != matchCount)
            {
                //после выхода из обработчика, попадаем в другой(в codebehind) который отменит выбранный дискрет
                return;
            }
            else
            {
                for (int i = 0; i < TempOutputKuSelected.Count; i++)
                {
                    this.TempOutputKuSelected[i] = OutputsKuSelected[i];
                }
            }
        }

        private void ManagementKuSelectedOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.GetBooleanArrForDiskrets();
            //var newmanagementKuSelected = new string[ManagementKuSelected.Count];
            if (!ValidateManagementKuSelection()) return;
            for (int i = 0; i < ManagementKuSelected.Count; i++)
            {
                TempManagmentCollection[i] = ManagementKuSelected[i];
            }
            return;
        }

        private void GetBooleanArrForDiskrets()
        {
            _diskretArrFromManagementKU.ClearAll();
            foreach (string kuSelected in ManagementKuSelected)
            {
                if (kuSelected != NO)
                    _diskretArrFromManagementKU[_managementNameValueDictionary[kuSelected] - 1] = true;
            }
        }



        private async void InitializingOnNavigateTo()
        {
            try
            {
                byte[] initializingData = null;
                byte[] versionData = null;

                initializingData = await this.ReadConfigurationDataFromDevice();
                if (initializingData == null)
                {
                    throw new Exception("Не удалось считать данные с устройства");
                }
                else
                {
                    MessageBox.Show("Чтение конфигурации прошло успешно", "Чтение конфигурации", MessageBoxButtons.OK);
                }
                this._isReadAndInitializeNow = true;
                this.InitializeFromReadingData(initializingData);

                this.VersionDescription = "Picon-2 ";
                this._isReadAndInitializeNow = false;
                if (this.TryInitializeFaultMask() != -1)
                {
                    this.ResetFaultBoxs();
                    throw new Exception(
                        @"Устройство сконфигурировано не верно. Пересечение масок неисправностей. 
                                                    Маски неисправностей будут сброшены.");
                }
            }
            catch (Exception error)
            {
                this.InteractWithError(error);
            }
        }

        private async void OnSendConfiguration()
        {
            try
            {

                var resultPackage = this.CreateDataPackage();


                await RTUConnectionGlobal.SendDataByAddressAsync(1, ADDRESS, ArrayExtension.ByteArrayToUshortArray(resultPackage.Take(LENGTH_WORD / 2).ToArray()));
                await RTUConnectionGlobal.SendDataByAddressAsync(1, ADDRESS + LENGTH_WORD / 4, ArrayExtension.ByteArrayToUshortArray(resultPackage.Skip(LENGTH_WORD / 2).Take(LENGTH_WORD / 2).ToArray()));
                await RTUConnectionGlobal.SendDataByAddressAsync(1, ADDRESS + LENGTH_WORD / 2, ArrayExtension.ByteArrayToUshortArray(resultPackage.Skip(LENGTH_WORD).Take(LENGTH_WORD / 2).ToArray()));
                await RTUConnectionGlobal.SendDataByAddressAsync(1, ADDRESS + LENGTH_WORD * 3 / 4, ArrayExtension.ByteArrayToUshortArray(resultPackage.Skip(LENGTH_WORD * 3 / 2).Take(LENGTH_WORD / 2).ToArray()));
                MessageBox.Show("Запись конфигурации прошла успешно", "Запись конфигурации", MessageBoxButtons.OK);
            }
            catch (Exception error)
            {
                this.InteractWithError(error);
            }
        }




        private void InteractWithError(Exception error)
        {
            MessageBox.Show(error.Message);
        }


        private async Task<byte[]> ReadConfigurationDataFromDevice()
        {
            byte[] result = ArrayExtension.UshortArrayToByteArray(await RTUConnectionGlobal.GetDataByAddress(1, ADDRESS, LENGTH_WORD));
            return result;
        }


        public void InitializeFromSettings(byte[] sett)
        {
            this.InitializeFromReadingData(sett);
            this.VersionDescription = "Picon-2 ";
            if (this.TryInitializeFaultMask() != -1)
            {
                this.ResetFaultBoxs();
                throw new Exception(
                    @"Устройство сконфигурировано не верно. Пересечение масок неисправностей. 
                                                    Маски неисправностей будут сброшены.");
            }
        }

        private void InitializeFromReadingData(byte[] data)
        {
            if (data.Length != LENGTH_WORD * 2)
            {
                throw new ArgumentException("Ошибка чтения попробуйте еще раз");
            }
            InitializeManagementChannels(data);
            InitializeKuOutput(data);
            InitializeManagementKuSelected(data);
            DeInitFaultPropertyChanged(FaultPower);
            DeInitFaultPropertyChanged(FaultManagement);
            DeInitFaultPropertyChanged(FaultSecurity);
            FaultPower = InitailizeMaskUnit(0x3030, data);
            FaultManagement = InitailizeMaskUnit(0x3034, data);
            FaultSecurity = InitailizeMaskUnit(0x3038, data);

            for (int i = 0; i < 8; i++)
            {
                DeInitFaultPropertyChanged(FaultCanals[i]);
                FaultCanals[i] = InitailizeMaskUnit(0x3040 + i * 8 / 2, data);
            }
            InitializeShedules(data);
            TimeToStart = GetUshortByAddress(data, 0x3014) / 10;
            ControllersNumber = GetUshortByAddress(data, 0x3013);
        }

        private int GetUshortByAddress(byte[] data, int address)
        {
            int offset = (address - ADDRESS) * 2;
            return BitConverter.ToInt16(new[] { data[offset + 1], data[offset] }, 0);
        }

        private void InitializeShedules(byte[] data)
        {
            int startShedulesAddress = 0x3015;
            int offset = (startShedulesAddress - ADDRESS) * 2;
            for (int i = 0; i < SheduleDates.Count; i++)
            {
                SheduleDates[i].DatePart1 = ConvertFromIntAsHexToIntAsDec(data[offset + i * 2]).ToString();
                SheduleDates[i].DatePart2 = ConvertFromIntAsHexToIntAsDec(data[offset + i * 2 + 1]).ToString();
            }
        }

        private PICON2ConfigCheckBoxControlViewModel InitailizeMaskUnit(int startAddress, byte[] data)
        {
            PICON2ConfigCheckBoxControlViewModel configCheckBoxControlViewModel = new PICON2ConfigCheckBoxControlViewModel();
            InitFaultPropertyChanged(configCheckBoxControlViewModel);
            int offset = (startAddress - ADDRESS) * 2;
            BitArray bitArray = new BitArray(new[]
            {
                data[offset + 1], data[offset], data[offset + 3], data[offset + 2], data[offset + 5], data[offset + 4],
                data[offset + 7], data[offset + 6]
            });
            for (int i = 0; i < configCheckBoxControlViewModel.CheckBoxResult.Count; i++)
            {
                configCheckBoxControlViewModel[i] = bitArray[i];
            }
            return configCheckBoxControlViewModel;
        }

        private void InitializeManagementKuSelected(byte[] data)
        {
            int startAddressKuOutput = 0x302c;
            int offset = (startAddressKuOutput - ADDRESS) * 2;
            for (int i = 0; i < ManagementKuSelected.Count; i++)
            {
                int inversionBytesCorrection = ((i % 2 == 0) || i == 0) ? 1 : -1;
                var dataValue = data[offset + i + inversionBytesCorrection];
                if (this._managementNameValueDictionary.ContainsValue(dataValue))
                    ManagementKuSelected[i] = this._managementNameValueDictionary
                        .First(pair => pair.Value.Equals(dataValue)).Key;
            }
        }

        private void InitializeKuOutput(byte[] data)
        {
            int startAddressKuOutput = 0x3020;
            int offset = (startAddressKuOutput - ADDRESS) * 2;
            for (int i = 0; i < OutputsKuSelected.Count; i++)
            {
                int inversionBytesCorrection = ((i % 2 == 0) || i == 0) ? 1 : -1;
                var dataValue = data[offset + i + inversionBytesCorrection];
                if (this._outputsNameValueDictionary.ContainsValue(dataValue))
                    OutputsKuSelected[i] = this._outputsNameValueDictionary.First(pair => pair.Value.Equals(dataValue))
                        .Key;
            }

            int startAddressKuOutputInv = 0x3024;
            int offsetInv = (startAddressKuOutputInv - ADDRESS) * 2;
            for (int i = 0; i < OutputsKuSelectedInv.Count; i++)
            {
                int inversionBytesCorrection = ((i % 2 == 0) || i == 0) ? 1 : -1;
                var dataValue = data[offsetInv + i + inversionBytesCorrection];
                if (this._outputsNameValueDictionary.ContainsValue(dataValue))
                    OutputsKuSelectedInv[i] = this._outputsNameValueDictionary
                        .First(pair => pair.Value.Equals(dataValue)).Key;
            }


        }


        private void InitializeManagementChannels(byte[] data)
        {
            int startAddressManagementChannels = 0x3028;
            int offset = (startAddressManagementChannels - ADDRESS) * 2;
            for (int i = 0; i < DataKuSelected.Count; i++)
            {
                int inversionBytesCorrection = ((i % 2 == 0) || i == 0) ? 1 : -1;
                var dataValue = data[offset + i + inversionBytesCorrection];
                if (this._dataNameValueDictionary.ContainsValue(dataValue))
                    DataKuSelected[i] = this._dataNameValueDictionary.First(pair => pair.Value.Equals(dataValue)).Key;
            }
        }



        /// <summary>
        ///     Создает пакет данных из выбранных позиций на вьюке
        ///     сделал пабликом для общего файла
        /// </summary>
        /// <returns>пакет данных для конфигурирования руно</returns>
        public byte[] CreateDataPackage()
        {
            var result = new byte[LENGTH_WORD * 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 0;
            }
            InitializeDataFromManagementChannels(result);
            InitializeDataFromKuOutput(result);
            InitializeDataFromManagementKuSelected(result);
            InitailizeDataFromMaskUnit(FaultPower, 0x3030, result);
            InitailizeDataFromMaskUnit(FaultManagement, 0x3034, result);
            InitailizeDataFromMaskUnit(FaultSecurity, 0x3038, result);

            for (int i = 0; i < 8; i++)
            {
                InitailizeDataFromMaskUnit(FaultCanals[i], 0x3040 + i * 8 / 2, result);
            }


            InitializeDataFromShedules(result);

            SetUshortToDataByAddress(result, 0x3014, TimeToStart * 10);
            SetUshortToDataByAddress(result, 0x3013, ControllersNumber);

            return result;
        }

        private void SetUshortToDataByAddress(byte[] result, int address, int value)
        {
            int offset = (address - ADDRESS) * 2;
            byte[] bytes = BitConverter.GetBytes((ushort)value);
            result[offset + 1] = bytes[0];
            result[offset] = bytes[1];
        }

        private void InitializeDataFromShedules(byte[] result)
        {
            int startShedulesAddress = 0x3015;
            int offset = (startShedulesAddress - ADDRESS) * 2;
            for (int i = 0; i < SheduleDates.Count; i++)
            {
                result[offset + i * 2] = ConvertFromIntAsDecToIntAsHex(byte.Parse(SheduleDates[i].DatePart1));
                result[offset + i * 2 + 1] = ConvertFromIntAsDecToIntAsHex(byte.Parse(SheduleDates[i].DatePart2));
            }
        }

        private void InitailizeDataFromMaskUnit(PICON2ConfigCheckBoxControlViewModel configCheckBoxControlViewModel, int address, byte[] result)
        {
            int offset = (address - ADDRESS) * 2;
            BitArray bitArray = new BitArray(configCheckBoxControlViewModel.GetCheckBoxBytes());
            byte[] bytes = ToByteArray(bitArray);//??????????????????????????????????????????????????????
            result[offset + 1] = bytes[0];
            result[offset] = bytes[1];
            result[offset + 3] = bytes[2];
            result[offset + 2] = bytes[3];
            result[offset + 5] = bytes[4];
            result[offset + 4] = bytes[5];
            result[offset + 7] = bytes[6];
            result[offset + 6] = bytes[7];

        }

        private void InitializeDataFromManagementKuSelected(byte[] result)
        {
            int startAddressKuOutput = 0x302c;
            int offset = (startAddressKuOutput - ADDRESS) * 2;
            for (int i = 0; i < ManagementKuSelected.Count; i++)
            {
                int inversionBytesCorrection = ((i % 2 == 0) || i == 0) ? 1 : -1;
                var dataValue = _managementNameValueDictionary[ManagementKuSelected[i]];
                result[offset + i + inversionBytesCorrection] = (byte)dataValue;
            }
        }

        private void InitializeDataFromKuOutput(byte[] result)
        {
            int startAddressKuOutput = 0x3020;
            int offset = (startAddressKuOutput - ADDRESS) * 2;
            for (int i = 0; i < OutputsKuSelected.Count; i++)
            {
                int inversionBytesCorrection = ((i % 2 == 0) || i == 0) ? 1 : -1;
                var dataValue = _outputsNameValueDictionary[OutputsKuSelected[i]];
                result[offset + i + inversionBytesCorrection] = (byte)dataValue;
            }

            int startAddressKuOutputInv = 0x3024;
            int offsetInv = (startAddressKuOutputInv - ADDRESS) * 2;
            for (int i = 0; i < OutputsKuSelectedInv.Count; i++)
            {
                int inversionBytesCorrection = ((i % 2 == 0) || i == 0) ? 1 : -1;
                var dataValue = _outputsNameValueDictionary[OutputsKuSelectedInv[i]];
                result[offsetInv + i + inversionBytesCorrection] = (byte)dataValue;
            }


        }

        private void InitializeDataFromManagementChannels(byte[] result)
        {
            int startAddressManagementChannels = 0x3028;
            int offset = (startAddressManagementChannels - ADDRESS) * 2;
            for (int i = 0; i < DataKuSelected.Count; i++)
            {
                int inversionBytesCorrection = ((i % 2 == 0) || i == 0) ? 1 : -1;
                var dataValue = _dataNameValueDictionary[DataKuSelected[i]];
                result[offset + i + inversionBytesCorrection] = (byte)dataValue;
            }

        }

        #endregion [Help Members]

        #region [Help Fault Validation]

        private void InitFaultPropertyChanged(PICON2ConfigCheckBoxControlViewModel configCheckBoxControlViewModel)
        {
            configCheckBoxControlViewModel.PropertyChanged += FaultOnPropertyChanged;
        }

        private void DeInitFaultPropertyChanged(PICON2ConfigCheckBoxControlViewModel configCheckBoxControlViewModel)
        {
            configCheckBoxControlViewModel.PropertyChanged -= FaultOnPropertyChanged;
        }
        private void FaultOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "CheckBoxResult" &&
                (sender is PICON2ConfigCheckBoxControlViewModel))
            {
                var viewModel = sender as PICON2ConfigCheckBoxControlViewModel;
                int faultedDiskret = this.TryInitializeFaultMask();
                if (faultedDiskret != -1)
                {
                    viewModel[faultedDiskret] = false;

                }
            }
        }

        private void ResetFaultBoxs()
        {
            for (int i = 0; i < 11; i++)
            {
                this.FaultManagement[i] = false;
                this.FaultPower[i] = false;
                this.FaultSecurity[i] = false;
                this.FaultMask[i] = false;
                foreach (var faultCanal in this.FaultCanals)
                {
                    faultCanal[i] = false;
                }
            }
        }

        /// <summary>
        ///     Инициализирует маску ошибок.
        /// </summary>
        /// <returns>если устройсво сконфигурировано неверно, возвращает номер дискрета, если верно -1</returns>
        private int TryInitializeFaultMask()
        {
            bool result = true;
            var tempMask = new bool[64];
            for (int i = 0; i < 64; i++)
            {
                if (this._diskretArrFromManagementKU[i] == true)
                {
                    if (tempMask[i] == true)
                    {
                        _diskretArrFromManagementKU[i] = false;
                        return i;
                    }
                    else
                    {
                        tempMask[i] = true;
                    }
                }


                if (this.FaultManagement[i] == true)
                {
                    if (tempMask[i] == true)
                    {
                        return i;
                    }
                    else
                    {
                        tempMask[i] = true;
                    }
                }
                if (this.FaultPower[i] == true)
                {
                    if (tempMask[i] == true)
                    {
                        return i;
                    }
                    else
                    {
                        tempMask[i] = true;
                    }
                }
                if (this.FaultSecurity[i] == true)
                {
                    if (tempMask[i] == true)
                    {
                        return i;
                    }
                    else
                    {
                        tempMask[i] = true;
                    }
                }
                foreach (var faultCanal in this.FaultCanals)
                {
                    if (faultCanal[i] == true)
                    {
                        if (tempMask[i] == true)
                        {
                            return i;
                        }
                        else
                        {
                            tempMask[i] = true;
                        }
                    }
                }
            }

            for (int i = 0; i < 64; i++)
            {
                this.FaultMask[i] = tempMask[i];
            }
            OnPropertyChanged("FaultMask");
            return -1;
        }

        public bool ValidateManagementKuSelection()
        {
            if (IsInitializeNow) return true;
            var newManagementKuCollection = ManagementKuSelected.Where(o => o != NO);

            var allDiskretCount = newManagementKuCollection.Count();
            var matchCount = newManagementKuCollection.Distinct().Count();
            //Если значения не равны, значит появилось совпадение
            if (allDiskretCount != matchCount)
            {
                return false;
            }

            return this.TryInitializeFaultMask() == -1;
        }






        #endregion [Help Fault Validation]



        // интересная история: в пиконе2 значения времени хранятся в хексе, 
        // но если читать это хексовое значение и предполагать, что на самом деле ты видишь десятичное, то это и будет действительное значение времени в десятичном варианте
        // то есть видишь 22 в хексе значит это 22 в дэке, но с устройства получаем уже преобразованные из хекса в дэк байты, поэтому так:
        private byte ConvertFromIntAsHexToIntAsDec(byte intAsHex)
        {
            string hexValueStr = intAsHex.ToString("X");
            byte decValue = Convert.ToByte(hexValueStr);
            return decValue;
        }

        // наоборот
        private byte ConvertFromIntAsDecToIntAsHex(byte intsAsDec)
        {
            string hexValueStr = intsAsDec.ToString("D");
            byte hexValue = Convert.ToByte(hexValueStr, 16);
            return hexValue;
        }


        private byte[] ToByteArray(BitArray bits)
        {
            int numBytes = bits.Count / 8;
            if (bits.Count % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << (bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }
    }
}