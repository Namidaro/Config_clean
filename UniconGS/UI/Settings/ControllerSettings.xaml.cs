using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using NModbus4.Device;
using UniconGS.Interfaces;
using UniconGS.Source;
using UniconGS.UI.Configuration;
using UniconGS.UI.GPRS;
using Table = UniconGS.UI.Schedule.Schedule;
using HeatingTable = UniconGS.UI.HeatingSchedule.HeatingSchedule;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using System.Windows.Controls.Primitives;
using UniconGS.UI.Schedule;
using UniconGS.Enums;
using System.Collections;

namespace UniconGS.UI.Settings
{
    /// <summary>
    /// Логика взаимодействия для ControllerSettings.xaml
    /// </summary>
    public partial class ControllerSettings
    {
        #region Fields
        public delegate void ShowMessageEventHandler(string message, string caption, MessageBoxImage image);
        public event ShowMessageEventHandler ShowMessage;
        private delegate void ReadCompleteDelegate(ushort[] res);
        private delegate void WriteCompleteDelegate(bool res);
        public delegate Settings GetControlsValueDelegate();
        public delegate Picon2Settings GetPicon2ControlsValueDelegate();
        public delegate void SetValueControlsDelegate(Settings settings);
        public delegate void SetPicon2ValueControlsDelegate(Picon2Settings picon2settings);
        public delegate void GetPicon2ModuleInfoDelegate();
        object x;
        public event GetControlsValueDelegate GetControlsValue;
        public event GetPicon2ControlsValueDelegate GetPicon2ControlsValue;
        public event SetValueControlsDelegate SetControlsValue;
        public event SetPicon2ValueControlsDelegate SetPicon2ControlsValue;
        public event GetPicon2ModuleInfoDelegate GetPicon2ModuleInfo;
        private Settings _settings;
        private Picon2Settings _picon2Settings;
        private bool _isWriteSettings;
        //private ushort[] _logicConfig;
        //private ushort[] _lightingSchedule;
        //private ushort[] _backlightSchedule;
        //private ushort[] _illuminationSchedule;
        //private ushort[] _conservationEnergySchedule;
        //private ushort[] _heatingSchedule;
        //private ushort[] _gprsConfig;
        #endregion

        public bool IsAutonomous { get; set; }
        public bool IsPicon2 { get; set; }
        public Config Config { get; set; }

        public ControllerSettings()
        {
            InitializeComponent();
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                uiPLCReset.IsEnabled = false;
                uiSignature.Visibility = Visibility.Collapsed;
                uiPicon2ModuleInfo.Visibility = Visibility.Visible;
                uiWriteAll.IsEnabled = false;
                uiReadAll.IsEnabled = true;
                IsPicon2 = true;
            }
            else
            {
                uiPLCReset.IsEnabled = true;
                uiSignature.IsEnabled = true;
                uiSignature.Visibility = Visibility.Visible;
                uiPicon2ModuleInfo.Visibility = Visibility.Collapsed;
                uiWriteAll.IsEnabled = true;
                uiReadAll.IsEnabled = true;
                IsPicon2 = false;
            }
        }
        public event EventHandler ReadAll;
        public event EventHandler WriteAll;
        //public void SetData(ushort[] s1, ushort[] s2, ushort[] s3, ushort[] s4, ushort[] s5, ushort[] s6, ushort[] s7)
        //{
        //    _lightingSchedule = s1;
        //    _backlightSchedule = s2;
        //    _illuminationSchedule = s3;
        //    _logicConfig = s4;
        //    _conservationEnergySchedule = s5;
        //    _heatingSchedule = s6;
        //    _gprsConfig = s7;
        //}
        #region Signature

        private async void uiSignature_Click(object sender, RoutedEventArgs e)
        {
            GetSignature();
        }

        private async void GetSignature()
        {
            uiSignature.IsEnabled = false;
            try
            {
                ushort[] res = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x0400), 52);
                ReadSignatureComplete(res);
            }
            catch (Exception exception)
            {

            }
            uiSignature.IsEnabled = true;
        }

        private string GetSignatureString(List<ushort> value)
        {
            ushort[] deviceName = value.GetRange(0, 4).ToArray();
            ushort[] version = value.GetRange(8, 3).ToArray();
            ushort[] date = value.GetRange(16, 5).ToArray();

            var devName = "Имя устройства: " + Converter.GetStringFromWords(deviceName) + ";\r\n";
            var v = "Версия:  " + ((byte)(version[1] >> 8)).ToString() + "."
                    + ((byte)version[1]).ToString() + "."
                    + ((byte)(version[0] >> 8)).ToString() + "."
                    + ((byte)version[0]).ToString() + ";\n\r";
            var d = "Дата: " + Converter.GetStringFromWords(date) + ".";

            return devName + v + d;
        }

        private void ReadSignatureComplete(ushort[] value)
        {
            if (value == null)
            {
                ShowMessage("Ошибка чтения сигнауры", "Сигнатура устройства.", MessageBoxImage.Error);
            }
            else
            {
                ShowMessage(this.GetSignatureString((value).ToList()), "Сигнатура устройства.",
                    MessageBoxImage.Information);
            }
            uiSignature.IsEnabled = true;
        }
        #endregion Signature

        #region PLC reset
        private async void uiPLCReset_Click(object sender, RoutedEventArgs e)
        {
                ResetPLC();
        }

        private async void ResetPLC()
        {
            //todo: PLC reset in picon2
            uiPLCReset.IsEnabled = false;
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                try
                {
                    await RTUConnectionGlobal.ExecuteFunction15Async(1, 0xFFFF, new bool[] { false,false, false, false, false, false, false, false,
                                                                                                false, false, false, false, false, false, false, false,
                                                                                                    false, false, false, false, false, false, false, false});
                    ShowMessage("Устройство было успешно сброшено.", "Внимание", MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ShowMessage("Во время сброса устройства произошла ошибка.", "Ошибка", MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(0x0302),
                        new ushort[] { 1 });
                    ShowMessage("Устройство было успешно сброшено.", "Внимание", MessageBoxImage.Information);
                }
                catch (Exception exception)
                {
                    ShowMessage("Во время сброса устройства произошла ошибка.", "Ошибка", MessageBoxImage.Error);
                }
            }
            uiPLCReset.IsEnabled = true;
        }
        #endregion

        #region Settings

        //private async void ApplySettings()
        //{
        //    if (!this.IsAutonomous)
        //    {
        //        if (this._settings == null)
        //        {
        //            ShowMessage("Настройки не импортированы", "Ошибка импорта настроек", MessageBoxImage.Error);
        //        }
        //        else
        //        {
        //        WriteSettings();
        //        }
        //    }
        //    else
        //    {
        //        ShowMessage("Невозможно записать настройки в устройство в автономном режиме.", "Автономный режим",
        //            MessageBoxImage.Exclamation);
        //    }
        //}

        private void uiSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            //todo: deal with setting file in picon2
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                SaveSettingsPicon2();
            }
            else
                SaveSettings();
        }
        private void SaveSettings()
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.InitialDirectory = this.Config.AllSettingsExportInitialFilePath;
            sfd.Filter = "Файл настройки Руно|*.gsset";
            sfd.Title = "Сохранение настроек \"Руно\"";
            sfd.FileName = "Настройки1";
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (this.GetAllSettings().Save(sfd.FileName))
            {
                this.Config.AllSettingsExportInitialFilePath = sfd.FileName;
                ShowMessage("Файл сохранен успешно", "Сохранение настроек", MessageBoxImage.Information);
            }
            else
            {
                ShowMessage("Во время сохранения файла произошла ошибка", "Ошибка сохранения настроек",
                    MessageBoxImage.Error);
            }
            uiSaveSettings.IsEnabled = true;
        }
        private void SaveSettingsPicon2()
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.InitialDirectory = this.Config.AllSettingsExportInitialFilePath;
            sfd.Filter = "Файл настройки Пикон2|*.p2sset";
            sfd.Title = "Сохранение настроек \"Пикон2\"";
            sfd.FileName = "Настройки1";
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (this.GetAllSettingsPicon2().Save(sfd.FileName))
            {
                this.Config.AllSettingsExportInitialFilePath = sfd.FileName;
                ShowMessage("Файл сохранен успешно", "Сохранение настроек", MessageBoxImage.Information);
            }
            else
            {
                ShowMessage("Во время сохранения файла произошла ошибка", "Ошибка сохранения настроек",
                    MessageBoxImage.Error);
            }
            uiSaveSettings.IsEnabled = true;
        }

        

        private void uiOpenSettings_Click(object sender, RoutedEventArgs e)
        {
            //todo:deal with settings file in picon 2
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                OpenSettingsPicon2();
            }
            else
                OpenSettings();
        }

        private void OpenSettings()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Файл настройки Руно|*.gsset",
                Title = "Открытие файла настройки \"Руно\"",
                InitialDirectory = this.Config.AllSettingsImportInitialFilePath
            };
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (System.IO.File.Exists(ofd.FileName))
            {
                if (this.SetAllSettings(Settings.Open(ofd.FileName)))
                {
                    this.Config.AllSettingsImportInitialFilePath = ofd.FileName;
                    SetControlsValue(_settings);
                    ShowMessage("Файл открыт успешно", "Открытие настроек", MessageBoxImage.Information);
                    //_isWriteSettings = true;
                    //this.ApplySettings();
                }
                else
                {
                    ShowMessage("Во время открытия файла произошла ошибка", "Ошибка открытия настроек",
                        MessageBoxImage.Error);
                }
            }
            else
            {
                ShowMessage("Выбранный файл не существует", "Ошибка открытия настроек", MessageBoxImage.Error);
            }
            uiOpenSettings.IsEnabled = true;
        }

        private void OpenSettingsPicon2()
        {
            var ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Файл настройки Пикон2|*.p2sset",
                Title = "Открытие файла настройки \"Пикон2\"",
                InitialDirectory = this.Config.AllSettingsImportInitialFilePath
            };
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (System.IO.File.Exists(ofd.FileName))
            {
                if (this.SetAllSettingsPicon2(Picon2Settings.Open(ofd.FileName)))
                {
                    this.Config.AllSettingsImportInitialFilePath = ofd.FileName;
                    SetPicon2ControlsValue(_picon2Settings);
                    ShowMessage("Файл открыт успешно", "Открытие настроек", MessageBoxImage.Information);
                    //_isWriteSettings = true;
                    //this.ApplySettings();
                }
                else
                {
                    ShowMessage("Во время открытия файла произошла ошибка", "Ошибка открытия настроек",
                        MessageBoxImage.Error);
                }
            }
            else
            {
                ShowMessage("Выбранный файл не существует", "Ошибка открытия настроек", MessageBoxImage.Error);
            }
            uiOpenSettings.IsEnabled = true;
        }

        private Settings GetAllSettings()
        {
            //this.Dispatcher.Invoke(new Action(() => RefreshControls()));
            this._settings = this.GetControlsValue();

            return this._settings;
        }
        private Picon2Settings GetAllSettingsPicon2()
        {
            //this.Dispatcher.Invoke(new Action(() => RefreshControls()));
            this._picon2Settings = this.GetPicon2ControlsValue();

            return this._picon2Settings;
        }

        private bool SetAllSettings(Settings settings)
        {

            if (settings == null)
            {

                return false;
            }
            this._settings = settings;

            return true;
        }
        private bool SetAllSettingsPicon2(Picon2Settings settings)
        {

            if (settings == null)
            {

                return false;
            }
            this._picon2Settings = settings;

            return true;
        }

        private async Task WriteSettings()
        {
            bool result = true;
            string message = string.Empty;

            try
            {




            }

            catch
            {
            }


            if (result)
                ShowMessage(@"Применение настроек прошло успешно." + "\r\n" + message, "Применение настроек",
                    MessageBoxImage.Information);
            else
                ShowMessage(@"Во время применения настроек произошла(и) ошибка(и)." + "\r\n" + message,
                    "Ошибка сохранения настроек", MessageBoxImage.Error);

        }


        #endregion Settings

        #region IQueryMember
        public ushort[] Value { get; set; }
        #endregion

        //private Table lightingSchedule;
        //private Table backlightSchedule;
        //private Table energySchedule;
        //private Table illuminationSchedule;

        private async void uiReadAll_Click(object sender, RoutedEventArgs e)
        {
            ReadAllConfig();
        }

        private void ReadAllConfig()
        {
            this.ReadAll?.Invoke(this, new EventArgs());
        }

        private async void uiWriteAll_Click(object sender, RoutedEventArgs e)
        {
            WriteAllConfig();
        }

        private void WriteAllConfig()
        {
            MessageBoxResult res = MessageBox.Show(
                "Внимание! Проверьте графики и конфигурацию перед записью в устройство. Вы уверены, что хотите совершить запись графиков и конфигурации в устройство?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.Yes)
            {
                if (this.WriteAll != null)
                {
                    this.WriteAll(this, new EventArgs());
                }
            }
            if (res == MessageBoxResult.No)
            {

            }
        }


        //public void Setup(Table uiLightingSchedule, Table uiBacklightSchedule, Table uiEnergySchedule, Table uiIlluminationSchedule)
        //{
        //    this.lightingSchedule = uiLightingSchedule;
        //    this.backlightSchedule = uiBacklightSchedule;
        //    this.energySchedule = uiEnergySchedule;
        //    this.illuminationSchedule = uiIlluminationSchedule;
        //}

        public void SetAutonomus()
        {
            this.uiPLCReset.IsEnabled = false;
            this.uiSignature.IsEnabled = false;
            this.uiPicon2ModuleInfo.IsEnabled = false;
            //            this.uiReadAll.IsEnabled = false;

            this.uiReadAll.IsEnabled = false;
            this.uiWriteAll.IsEnabled = false;
        }

        public void DisableAutonomus()
        {
            this.uiPLCReset.IsEnabled = true;
            this.uiSignature.IsEnabled = true;
            this.uiPicon2ModuleInfo.IsEnabled = true;
            //            this.uiReadAll.IsEnabled = false;

            this.uiReadAll.IsEnabled = true;
            this.uiWriteAll.IsEnabled = true;
        }

        private void uiPicon2ModuleInfo_Click(object sender, RoutedEventArgs e)
        {
            GetPicon2ModuleInfo.Invoke();
        }

    }
}