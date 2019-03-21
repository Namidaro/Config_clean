using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UniconGS.Enums;

namespace UniconGS.UI.Configuration
{
    /// <summary>
    /// Interaction logic for ErrorMask.xaml
    /// </summary>
    public partial class ErrorMask : UserControl
    {
        public delegate void ChekedChangedEventHandler(object sender, int index, bool value);
        public event ChekedChangedEventHandler ChekedChanged;

        private List<CheckBox> _cells = new List<CheckBox>();

        public object BindableValue
        {
            get
                
            {
                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                {
                    PART_Holder.Height = 39.259;
                    PART_Holder.Width = 275.22;
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                {
                    PART_Holder.Height = 91.259;
                    PART_Holder.Width = 275.22;
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                {
                    PART_Holder.Height = 91.259;
                    PART_Holder.Width = 275.22;
                }
                return this.PART_Holder.DataContext;
            }
            set
            {
                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                {
                    PART_Holder.Height = 39.259;
                    PART_Holder.Width = 275.22;
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                {
                    PART_Holder.Height = 91.259;
                    PART_Holder.Width = 275.22;
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                {
                    PART_Holder.Height = 91.259;
                    PART_Holder.Width = 275.22;
                }
                this.PART_Holder.DataContext = value;
            }
        }

        public ErrorMask()
        {
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
            {
               

                InitializeComponent();
                this._cells = new List<CheckBox>() 
                 {
                this.ui_0,
                this.ui_1,
                this.ui_2,
                this.ui_3,
                this.ui_4,
                this.ui_5,
                this.ui_6,
                this.ui_7,
                this.ui_8,
                this.ui_9,
                this.ui_10
                };
                this.uiBlock1.Visibility = Visibility.Hidden;
                this.uiBlock2.Visibility = Visibility.Hidden;
                this.uiBlock3.Visibility = Visibility.Hidden;
            }

            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
            {

               InitializeComponent();
                this._cells = new List<CheckBox>() 
                 {
                this.ui_0,
                this.ui_1,
                this.ui_2,
                this.ui_3,
                this.ui_4,
                this.ui_5,
                this.ui_6,
                this.ui_7,
                this.ui_8,
                this.ui_9,
                this.ui_10,
                this.ui_11,
                this.ui_12,
                this.ui_13,
                this.ui_14,
                this.ui_15,
                this.ui_16,
                this.ui_17,
                this.ui_18,
                this.ui_19,
                this.ui_20,
                this.ui_21,
                this.ui_22,
                this.ui_23,
                this.ui_24,
                this.ui_25,
                this.ui_26,
                this.ui_27,
                this.ui_28,
                this.ui_29,
                this.ui_30,
                this.ui_31,
                this.ui_32,
                this.ui_33,
                this.ui_34,
                this.ui_35,
                this.ui_36,
                this.ui_37,
                this.ui_38,
                this.ui_39,
                this.ui_40,
                this.ui_41,
                this.ui_42,
                this.ui_43
                };
                this.uiBlock1.Visibility = Visibility.Visible;
                this.uiBlock2.Visibility = Visibility.Visible;
                this.uiBlock3.Visibility = Visibility.Visible;
                this.ui_0.Visibility = Visibility.Visible;
                this.ui_1.Visibility = Visibility.Visible;
                this.ui_2.Visibility = Visibility.Visible;
                this.ui_3.Visibility = Visibility.Visible;
                this.ui_4.Visibility = Visibility.Visible;
                this.ui_5.Visibility = Visibility.Visible;
                this.ui_6.Visibility = Visibility.Visible;
                this.ui_7.Visibility = Visibility.Visible;
                this.ui_8.Visibility = Visibility.Visible;
                this.ui_9.Visibility = Visibility.Visible;
                this.ui_10.Visibility = Visibility.Visible;
                this.ui_11.Visibility = Visibility.Visible;
                this.ui_12.Visibility = Visibility.Visible;
                this.ui_13.Visibility = Visibility.Visible;
                this.ui_14.Visibility = Visibility.Visible;
                this.ui_15.Visibility = Visibility.Visible;
                this.ui_16.Visibility = Visibility.Visible;
                this.ui_17.Visibility = Visibility.Visible;
                this.ui_18.Visibility = Visibility.Visible;
                this.ui_19.Visibility = Visibility.Visible;
                this.ui_20.Visibility = Visibility.Visible;
                this.ui_21.Visibility = Visibility.Visible;
                this.ui_22.Visibility = Visibility.Visible;
                this.ui_23.Visibility = Visibility.Visible;
                this.ui_24.Visibility = Visibility.Visible;
                this.ui_25.Visibility = Visibility.Visible;
                this.ui_26.Visibility = Visibility.Visible;
                this.ui_27.Visibility = Visibility.Visible;
                this.ui_28.Visibility = Visibility.Visible;
                this.ui_29.Visibility = Visibility.Visible;
                this.ui_30.Visibility = Visibility.Visible;
                this.ui_31.Visibility = Visibility.Visible;
                this.ui_32.Visibility = Visibility.Visible;
                this.ui_33.Visibility = Visibility.Visible;
                this.ui_34.Visibility = Visibility.Visible;
                this.ui_35.Visibility = Visibility.Visible;
                this.ui_36.Visibility = Visibility.Visible;
                this.ui_37.Visibility = Visibility.Visible;
                this.ui_38.Visibility = Visibility.Visible;
                this.ui_39.Visibility = Visibility.Visible;
                this.ui_40.Visibility = Visibility.Visible;
                this.ui_41.Visibility = Visibility.Visible;
                this.ui_42.Visibility = Visibility.Visible;
                this.ui_43.Visibility = Visibility.Visible;
            }

            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {


                InitializeComponent();
                this._cells = new List<CheckBox>()
                {
                    this.ui_0,
                    this.ui_1,
                    this.ui_2,
                    this.ui_3,
                    this.ui_4,
                    this.ui_5,
                    this.ui_6,
                    this.ui_7,
                    this.ui_8,
                    this.ui_9,
                    this.ui_10
                };
                this.uiBlock1.Visibility = Visibility.Hidden;
                this.uiBlock2.Visibility = Visibility.Hidden;
                this.uiBlock3.Visibility = Visibility.Hidden;
            }
            ;

        }

        public void SetEnabled(int index)
        {
            this._cells[index].IsEnabled = true;
        }

        public void SetDisable(int index)
        {
            this._cells[index].IsEnabled = false;
            this._cells[index].IsChecked = false;
           

        }

        public void SetAutonomus()
        {
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
            {

                this.uiBlock1.Visibility = Visibility.Hidden;
                this.uiBlock2.Visibility = Visibility.Hidden;
                this.uiBlock3.Visibility = Visibility.Hidden;
                this.ui_0.Visibility = Visibility.Visible;
                this.ui_1.Visibility = Visibility.Visible;
                this.ui_2.Visibility = Visibility.Visible;
                this.ui_3.Visibility = Visibility.Visible;
                this.ui_4.Visibility = Visibility.Visible;
                this.ui_5.Visibility = Visibility.Visible;
                this.ui_6.Visibility = Visibility.Visible;
                this.ui_7.Visibility = Visibility.Visible;
                this.ui_8.Visibility = Visibility.Visible;
                this.ui_9.Visibility = Visibility.Visible;
                this.ui_10.Visibility = Visibility.Visible;
            }
            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
            {
               
                this.uiBlock1.Visibility = Visibility.Visible;
                this.uiBlock2.Visibility = Visibility.Visible;
                this.uiBlock3.Visibility = Visibility.Visible;
                this.ui_0.Visibility = Visibility.Visible;
                this.ui_1.Visibility = Visibility.Visible;
                this.ui_2.Visibility = Visibility.Visible;
                this.ui_3.Visibility = Visibility.Visible;
                this.ui_4.Visibility = Visibility.Visible;
                this.ui_5.Visibility = Visibility.Visible;
                this.ui_6.Visibility = Visibility.Visible;
                this.ui_7.Visibility = Visibility.Visible;
                this.ui_8.Visibility = Visibility.Visible;
                this.ui_9.Visibility = Visibility.Visible;
                this.ui_10.Visibility = Visibility.Visible;
                this.ui_11.Visibility = Visibility.Visible;
                this.ui_12.Visibility = Visibility.Visible;
                this.ui_13.Visibility = Visibility.Visible;
                this.ui_14.Visibility = Visibility.Visible;
                this.ui_15.Visibility = Visibility.Visible;
                this.ui_16.Visibility = Visibility.Visible;
                this.ui_17.Visibility = Visibility.Visible;
                this.ui_18.Visibility = Visibility.Visible;
                this.ui_19.Visibility = Visibility.Visible;
                this.ui_20.Visibility = Visibility.Visible;
                this.ui_21.Visibility = Visibility.Visible;
                this.ui_22.Visibility = Visibility.Visible;
                this.ui_23.Visibility = Visibility.Visible;
                this.ui_24.Visibility = Visibility.Visible;
                this.ui_25.Visibility = Visibility.Visible;
                this.ui_26.Visibility = Visibility.Visible;
                this.ui_27.Visibility = Visibility.Visible;
                this.ui_28.Visibility = Visibility.Visible;
                this.ui_29.Visibility = Visibility.Visible;
                this.ui_30.Visibility = Visibility.Visible;
                this.ui_31.Visibility = Visibility.Visible;
                this.ui_32.Visibility = Visibility.Visible;
                this.ui_33.Visibility = Visibility.Visible;
                this.ui_34.Visibility = Visibility.Visible;
                this.ui_35.Visibility = Visibility.Visible;
                this.ui_36.Visibility = Visibility.Visible;
                this.ui_37.Visibility = Visibility.Visible;
                this.ui_38.Visibility = Visibility.Visible;
                this.ui_39.Visibility = Visibility.Visible;
                this.ui_40.Visibility = Visibility.Visible;
                this.ui_41.Visibility = Visibility.Visible;
                this.ui_42.Visibility = Visibility.Visible;
                this.ui_43.Visibility = Visibility.Visible;
            }
            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {

                this.uiBlock1.Visibility = Visibility.Hidden;
                this.uiBlock2.Visibility = Visibility.Hidden;
                this.uiBlock3.Visibility = Visibility.Hidden;
                this.ui_0.Visibility = Visibility.Visible;
                this.ui_1.Visibility = Visibility.Visible;
                this.ui_2.Visibility = Visibility.Visible;
                this.ui_3.Visibility = Visibility.Visible;
                this.ui_4.Visibility = Visibility.Visible;
                this.ui_5.Visibility = Visibility.Visible;
                this.ui_6.Visibility = Visibility.Visible;
                this.ui_7.Visibility = Visibility.Visible;
                this.ui_8.Visibility = Visibility.Visible;
                this.ui_9.Visibility = Visibility.Visible;
                this.ui_10.Visibility = Visibility.Visible;
            }
        }
       

        public bool isChecked(int index)
        {
            return this._cells[index].IsChecked.Value;
        }

        public void ClearAll()
        {
            foreach (var item in this._cells)
            {
                item.IsChecked = false;
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (this.ChekedChanged != null)

            {
                 this.ChekedChanged(this, int.Parse((sender as CheckBox).Name.Replace("ui_", "")), (sender as CheckBox).IsChecked.Value);
            }
        }
    }
}
