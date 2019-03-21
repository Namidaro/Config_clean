using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UniconGS.Annotations;
using UniconGS.Interfaces;
using UniconGS.Source;
using UniconGS.Enums;

namespace UniconGS.UI.Configuration
{
    /// <summary>
    /// Interaction logic for LogicConfig.xaml
    /// </summary>
    public partial class LogicConfig : UserControl
    {
        #region Events
        public delegate object OpenFromFileEventHandler(Type type);
        public event OpenFromFileEventHandler OpenFromFile;

        public delegate void SaveInFileEventHandler(object value, Type type);
        public event SaveInFileEventHandler SaveInFile;

        public delegate void ShowMessageEventHandler(string message, string caption, MessageBoxImage image);
        public event ShowMessageEventHandler ShowMessage;

        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();
        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;

        private delegate void ReadComplete(ushort[] res);
        private delegate void WriteComplete(bool res);
        #endregion

        #region Globals
        private ChannelManagment _value = new ChannelManagment();
        private Slot _query;
        private List<ComboBox> _kUList = new List<ComboBox>();
        private List<ErrorMask> _maswks = new List<ErrorMask>();
        private bool _afterError;
        #endregion



        public LogicConfig()
        {
            InitializeComponent();
            this._value.InitializeChannelByDefault();
            this.SetComboBoxItems();
            this.UpdateBinding();
            this.InitializeEvents();
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
            {
                Mask.Height = 70.333;
            }
            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
            {
                Mask.Height = 125.333;
            }
            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                Mask.Height = 125.333;
            }


            this._kUList = new List<ComboBox>()
            {
                this.uiKU1,
                this.uiKU2,
                this.uiKU3,
                this.uiKU4,
                this.uiKU5,
                this.uiKU6,
                this.uiKU7,
                this.uiKU8
            };

            this._maswks = new List<ErrorMask>()
            {
                this.uiChannel1Matrix,
                this.uiChannel2Matrix,
                this.uiChannel3Matrix,
                this.uiChannel4Matrix,
                this.uiChannel5Matrix,
                this.uiChannel6Matrix,
                this.uiChannel7Matrix,
                this.uiChannel8Matrix,
                this.uiManagmentMatrix,
                this.uiPowerMatrix,
                this.uiSecurityMatrix
            };
        }



        public void SetAutonomous()
        {
            this.uiLogicExport.IsEnabled = false;
            this.uiLogicImport.IsEnabled = false;
        }

        public void DisableAutonomous()
        {
            this.uiLogicExport.IsEnabled = true;
            this.uiLogicImport.IsEnabled = true;
        }

        public bool ValidateAutomationTime()
        {
            var result = true;
            try
            {
                var value = int.Parse(this.uiAutomationTime.Text);
                if (value < 60 | value > 1000)
                    throw new Exception();
            }
            catch (Exception)
            {
                //result = false;
            }
            return result;
        }




        void uiKU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemIndex = (sender as ComboBox).SelectedIndex;
            var kuNum = int.Parse((sender as ComboBox).Name.Replace("uiKU", ""));

            if (itemIndex != 0)
            {
                //1 проверка на каналах
                var resultKU = false;
                foreach (var item in this._kUList)
                {
                    if (!item.Equals(sender) && item.SelectedIndex == (sender as ComboBox).SelectedIndex)
                    {
                        resultKU = true;
                        break;
                    }
                }
                //2 проверка на масках
                if (!resultKU)
                {
                    foreach (var item in this._maswks)
                    {
                        if (item.isChecked(itemIndex - 1))
                        {
                            resultKU = true;
                            break;
                        }
                    }
                }
                if (resultKU)
                {
                    MessageBox.Show("Контроль управления уже занят", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    var prevIndex = (sender as ComboBox).Items.IndexOf(e.RemovedItems[0]);
                    if (prevIndex != 0 & prevIndex != -1)
                        this._afterError = true;
                    (sender as ComboBox).SelectedItem = e.RemovedItems[0];
                }
                else
                {
                    if (!this._value.ErrorMask.Value[itemIndex - 1])
                    {
                        this.UpdateMatrixFromErrorMatrix(itemIndex - 1, true);
                        if (e.RemovedItems.Count != 0)
                        {
                            var prevIndex = (sender as ComboBox).Items.IndexOf(e.RemovedItems[0]);
                            if (prevIndex != 0)
                                this.UpdateMatrixFromErrorMatrix(prevIndex - 1, false);
                        }
                    }
                    this._afterError = false;
                }
            }
            else
            {
                if (e.RemovedItems.Count != 0)
                {
                    var prevIndex = (sender as ComboBox).Items.IndexOf(e.RemovedItems[0]);
                    var resultKU = false;
                    if (this._afterError)
                    {
                        foreach (var item in this._kUList)
                        {
                            if (!item.Equals(sender) && item.SelectedIndex == prevIndex - 1)
                            {
                                resultKU = true;
                                break;
                            }
                        }
                        if (!resultKU)
                            this.UpdateMatrixFromErrorMatrix(prevIndex - 1, false);
                    }
                    else
                        this.UpdateMatrixFromErrorMatrix(prevIndex - 1, false);
                    if (e.RemovedItems.Count != 0 && ((sender as ComboBox).Items.IndexOf(e.RemovedItems[0]) - 1) != -1)
                    {
                        if (!this.uiErrorMatrix.isChecked((sender as ComboBox).Items.IndexOf(e.RemovedItems[0]) - 1))
                        {
                            this.MatrixUpdate();
                            if (e.RemovedItems.Count != 0)
                            {
                                foreach (var item in this._maswks)
                                {
                                    item.SetEnabled((sender as ComboBox).Items.IndexOf(e.RemovedItems[0]) - 1);
                                }
                            }
                        }
                        else
                        {
                            this.MatrixUpdate();

                            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                            {
                                for (int i = 0; i < 11; i++)
                                {
                                    if (this.uiErrorMatrix.isChecked(i))
                                    {
                                        foreach (var item in this._maswks)
                                        {
                                            if (!item.isChecked(i))
                                                item.SetDisable(i);
                                        }
                                    }
                                }
                            }

                            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                            {
                                for (int i = 0; i < 44; i++)
                                {
                                    if (this.uiErrorMatrix.isChecked(i))
                                    {
                                        foreach (var item in this._maswks)
                                        {
                                            if (!item.isChecked(i))
                                                item.SetDisable(i);
                                        }
                                    }
                                }
                            }
                            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                            {
                                for (int i = 0; i < 11; i++)
                                {
                                    if (this.uiErrorMatrix.isChecked(i))
                                    {
                                        foreach (var item in this._maswks)
                                        {
                                            if (!item.isChecked(i))
                                                item.SetDisable(i);
                                        }
                                    }
                                }
                            }


                        }
                    }
                    this._afterError = false;
                }
            }

        }

        private void UpdateMatrixFromErrorMatrix(int index, bool value)
        {
            var result = false;

            foreach (var item in this._maswks)
            {
                if (item.isChecked(index))
                {
                    result = true;
                    break;
                }
            }

            if (!result)
            {
                this._value.ErrorMask.Value[index] = value;
            }

            if (value)
            {
                foreach (var item in this._maswks)
                {
                    item.SetDisable(index);
                }
            }
            else
            {
                if (!result)
                {
                    foreach (var item in this._maswks)
                    {
                        item.SetEnabled(index);
                    }
                }
            }
        }

        public async Task UpdateState()
        {

            uiLogicConfigOpen.IsEnabled =
                uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = false;
            ushort[] value;
            {
                value = await RTUConnectionGlobal.GetDataByAddress(1, 0x8200, 61);
            }

            ImportComplete(value);

            uiLogicConfigOpen.IsEnabled =
                uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = true;
        }
        public async void uiLogicImport_Click(object sender, RoutedEventArgs e)
        {
            await UpdateState();
            if (this.ShowMessage != null)
            {
                this.ShowMessage("Чтение конфигурации логики прошло успешно",
                       "Чтение конфигурации логики", MessageBoxImage.Information);
            }


        }

        private void ImportComplete(ushort[] value)
        {
            if (value != null)
            {
                this.ClearAll();
                this._value.SetData(value);
                this.UpdateBinding();
                this.MatrixUpdate();
                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        if (this.uiErrorMatrix.isChecked(i))
                        {
                            foreach (var item in this._maswks)
                            {
                                if (!item.isChecked(i))
                                    item.SetDisable(i);
                            }
                        }
                    }
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                {

                    for (int i = 0; i < 44; i++)
                    {
                        if (this.uiErrorMatrix.isChecked(i))
                        {
                            foreach (var item in this._maswks)
                            {
                                if (!item.isChecked(i))
                                    item.SetDisable(i);
                            }
                        }
                    }
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                {

                    for (int i = 0; i < 44; i++)
                    {
                        if (this.uiErrorMatrix.isChecked(i))
                        {
                            foreach (var item in this._maswks)
                            {
                                if (!item.isChecked(i))
                                    item.SetDisable(i);
                            }
                        }
                    }
                }

            }
            else
            {
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время чтения конфигурации логики из устройства произошла ошибка.",
                        "Чтение конфигурации логики", MessageBoxImage.Error);
                }
            }
            uiLogicConfigOpen.IsEnabled =
                uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = true;
        }
        public async void uiLogicExport_Click(object sender, RoutedEventArgs e)
        {

            await WriteAll();
            if (this.ShowMessage != null)
            {
                this.ShowMessage("Запись конфигурации логики в устройство прошла успешно.",
                    "Запись конфигурации логики в устройство", MessageBoxImage.Information);
            }

        }

        public async Task WriteAll()
        {
            bool res;
            res = false;
            if (this.ValidateAutomationTime())
            {
                uiLogicConfigOpen.IsEnabled =
                    uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = false;

                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x8200, Value);
                    res = true;
                }
                ExportComplete(res);
                uiLogicConfigOpen.IsEnabled =
                    uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = true;
                //DataTransfer.SetTopInQueue(this, Accsess.Write, false);
            }
            else
            {
                this.ShowMessage(
                    "Не верное значение времени перехода в автоматический режим. Значение должно быть в пределах [5;1800]",
                    "Запись конфигурации логики в устройство", MessageBoxImage.Warning);

            }
        }

        public void ExportComplete(bool res)
        {
            if (res)
            {

            }
            else
            {
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время записи конфигурации логики в устройство произошла ошибка.",
                        "Запись конфигурации логики в устройство", MessageBoxImage.Error);
                }
            }
            uiLogicConfigOpen.IsEnabled =
                uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = true;
        }

        private void uiLogicConfigSave_Click(object sender, RoutedEventArgs e)
        {
            uiLogicConfigOpen.IsEnabled =
                uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = false;
            if (this.ValidateAutomationTime())
            {
                if (this.SaveInFile != null)
                {
                    this.SaveInFile(this._value, typeof(ChannelManagment));
                }
            }
            else
            {
                this.ShowMessage(
                    "Не верное значение времени перехода в автоматический режим. Значение должно быть в пределах [5;1800]",
                    "Запись конфигурации логики в устройство", MessageBoxImage.Warning);
            }
            if (MainWindow.isAutonomus == true)
            {
                uiLogicConfigOpen.IsEnabled = uiLogicConfigSave.IsEnabled = true;
                uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = false;
            }
            else
            {
                uiLogicConfigOpen.IsEnabled =
                    uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = true;
            }

        }

        private void uiLogicConfigOpen_Click(object sender, RoutedEventArgs e)
        {
            uiLogicConfigOpen.IsEnabled =
                uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = false;
            if (this.OpenFromFile != null)
            {
                var result = this.OpenFromFile(typeof(ChannelManagment));
                if (result != null)
                {
                    this.ClearAll();
                    if (this.StartWork != null)
                        this.StartWork();
                    this._value = (ChannelManagment)result;
                    this.UpdateBinding();
                    this.MatrixUpdate();
                    if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                    {
                        for (int i = 0; i < 11; i++)
                        {
                            if (this.uiErrorMatrix.isChecked(i))
                            {
                                foreach (var item in this._maswks)
                                {
                                    if (!item.isChecked(i))
                                        item.SetDisable(i);
                                }
                            }
                        }
                    }
                    else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                    {
                        for (int i = 0; i < 44; i++)
                        {
                            if (this.uiErrorMatrix.isChecked(i))
                            {
                                foreach (var item in this._maswks)
                                {
                                    if (!item.isChecked(i))
                                        item.SetDisable(i);
                                }
                            }
                        }
                    }
                    else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                    {

                        for (int i = 0; i < 44; i++)
                        {
                            if (this.uiErrorMatrix.isChecked(i))
                            {
                                foreach (var item in this._maswks)
                                {
                                    if (!item.isChecked(i))
                                        item.SetDisable(i);
                                }
                            }
                        }
                    }
                    if (this.StopWork != null)
                        this.StopWork();
                }
            }
            if (MainWindow.isAutonomus == true)
            {
                uiLogicConfigOpen.IsEnabled = uiLogicConfigSave.IsEnabled = true;
                uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = false;
            }
            else
            {
                uiLogicConfigOpen.IsEnabled =
                    uiLogicConfigSave.IsEnabled = uiLogicExport.IsEnabled = uiLogicImport.IsEnabled = true;
            }

        }

        private void ClearAll()
        {
            foreach (var item in this._kUList)
            {
                item.SelectedIndex = 0;
            }
            foreach (var item in this._maswks)
            {
                item.ClearAll();
            }
            this.MatrixUpdate();


        }

        private void InitializeEvents()
        {
            this.uiChannel1Matrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiChannel2Matrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiChannel3Matrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiChannel4Matrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiChannel5Matrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiChannel6Matrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiChannel7Matrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiChannel8Matrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiPowerMatrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiManagmentMatrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
            this.uiSecurityMatrix.ChekedChanged += new ErrorMask.ChekedChangedEventHandler(MatrixUpdate);
        }
        private void MatrixUpdate(int index)
        {
            bool result = false;
            foreach (var item in this._value.ChannelMasks)
            {
                result |= item.Value[index];
            }
            result |= this._value.ManagmentMask.Value[index] | this._value.PowerMask.Value[index] | this._value.SecurityMask.Value[index];
            this._value.ErrorMask.Value[index] = result;
        }


        private void MatrixUpdate(object sender, int index, bool value)
        {
            for (int i = 0; i < this._maswks.Count; i++)
            {
                if (value)
                {
                    if (!this._maswks[i].Equals(sender))
                        this._maswks[i].SetDisable(index);
                }
                else
                {
                    if (!this._maswks[i].Equals(sender))
                        this._maswks[i].SetEnabled(index);
                }
            }

            bool result = false;
            foreach (var item in this._value.ChannelMasks)
            {
                result |= item.Value[index];
            }
            result |= this._value.ManagmentMask.Value[index] |
                      this._value.PowerMask.Value[index] |
                      this._value.SecurityMask.Value[index];

            this._value.ErrorMask.Value[index] = result;
        }
        private void MatrixUpdate()
        {
            for (int i = 0; i < 44; i++)
            {
                this.MatrixUpdate(i);
            }
            for (int i = 0; i < this._kUList.Count; i++)
            {
                if (this._kUList[i].SelectedIndex > 0)
                    this._value.ErrorMask.Value[this._kUList[i].SelectedIndex - 1] = true;
            }
        }

        public void UpdateBinding()
        {
            this.uiChannel1Matrix.BindableValue = this._value.ChannelMasks[0].Value;
            this.uiChannel2Matrix.BindableValue = this._value.ChannelMasks[1].Value;
            this.uiChannel3Matrix.BindableValue = this._value.ChannelMasks[2].Value;
            this.uiChannel4Matrix.BindableValue = this._value.ChannelMasks[3].Value;
            this.uiChannel5Matrix.BindableValue = this._value.ChannelMasks[4].Value;
            this.uiChannel6Matrix.BindableValue = this._value.ChannelMasks[5].Value;
            this.uiChannel7Matrix.BindableValue = this._value.ChannelMasks[6].Value;
            this.uiChannel8Matrix.BindableValue = this._value.ChannelMasks[7].Value;
            this.uiPowerMatrix.BindableValue = this._value.PowerMask.Value;
            this.uiManagmentMatrix.BindableValue = this._value.ManagmentMask.Value;
            this.uiSecurityMatrix.BindableValue = this._value.SecurityMask.Value;
            this.PART_AutomationHolder.DataContext = this._value;
            this.uiErrorMatrix.BindableValue = this._value.ErrorMask.Value;
            this.PART_LogicContainer.DataContext = this._value.Channels;
            this.uiAutomationTime.DataContext = this._value;
        }

        private void SetComboBoxItems()
        {

            this.uiDataKU1.ItemsSource = ComboBoxItemStr.DataKU;
            this.uiDataKU2.ItemsSource = ComboBoxItemStr.DataKU;
            this.uiDataKU3.ItemsSource = ComboBoxItemStr.DataKU;
            this.uiDataKU4.ItemsSource = ComboBoxItemStr.DataKU;
            this.uiDataKU5.ItemsSource = ComboBoxItemStr.DataKU;
            this.uiDataKU6.ItemsSource = ComboBoxItemStr.DataKU;
            this.uiDataKU7.ItemsSource = ComboBoxItemStr.DataKU;
            this.uiDataKU8.ItemsSource = ComboBoxItemStr.DataKU;

            this.uiOutKU1.ItemsSource = ComboBoxItemStr.OutputKU;
            this.uiOutKU2.ItemsSource = ComboBoxItemStr.OutputKU;
            this.uiOutKU3.ItemsSource = ComboBoxItemStr.OutputKU;
            this.uiOutKU4.ItemsSource = ComboBoxItemStr.OutputKU;
            this.uiOutKU5.ItemsSource = ComboBoxItemStr.OutputKU;
            this.uiOutKU6.ItemsSource = ComboBoxItemStr.OutputKU;
            this.uiOutKU7.ItemsSource = ComboBoxItemStr.OutputKU;
            this.uiOutKU8.ItemsSource = ComboBoxItemStr.OutputKU;

            this.uiKU1.ItemsSource = ComboBoxItemStr.Control;
            this.uiKU2.ItemsSource = ComboBoxItemStr.Control;
            this.uiKU3.ItemsSource = ComboBoxItemStr.Control;
            this.uiKU4.ItemsSource = ComboBoxItemStr.Control;
            this.uiKU5.ItemsSource = ComboBoxItemStr.Control;
            this.uiKU6.ItemsSource = ComboBoxItemStr.Control;
            this.uiKU7.ItemsSource = ComboBoxItemStr.Control;
            this.uiKU8.ItemsSource = ComboBoxItemStr.Control;
        }


        public ushort[] Value
        {
            get
            {
                return this._value.GetValue();
            }
            set
            {
                if (value != null)
                {
                    if (value is Array)
                    {
                        this._value.SetData(value);
                    }
                }
            }
        }

    }
}
