using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UniconGS.Annotations;
using UniconGS.Source;
using UniconGS.UI;

namespace UniconGS
{
    /// <summary>
    /// Логика взаимодействия для Luxmetr.xaml
    /// </summary>
    public partial class Luxmetr : Window 
    {
        private Thread _work;
        private int _updateRate = 1;
        private ManualResetEvent _shutDownEvent = new ManualResetEvent(true);
        private Slot _connectionChecker;


        private void DoWork()
        {
            while (true)
            {
                if (this._shutDownEvent.WaitOne(0))
                    return;

                //this.ReadWriteData();

                /*Цикличное чтение имени устройства для определения состояния связи*/
                if (!this.GetConnectionState())
                {
                    this.Dispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(this.ConnectionLost));
                }
                else
                {
                   
                }
                Thread.Sleep(this._updateRate);
            }
        }


        public void TabItemSelected(object sender, SelectionChangedEventArgs e)
        {
            int tab = (sender as TabControl).SelectedIndex;
            switch (tab)
            {
                case 1:
                    try
                    {
                        if (DataTransfer.QueryQueue != null)
                        {
                            var temp =
                                DataTransfer.QueryQueue.ToArray()
                                    .Where(q => q.IsCycle == false)
                                    .Select(q => q)
                                    .ToArray();
                            DataTransfer.QueryQueue.Clear();
                            foreach (var t in temp)
                            {
                                DataTransfer.QueryQueue.Enqueue(t);
                            }
                            //DataTransfer.QueryQueue.Enqueue(new Query(uiLightMeasurement, true, Accsess.Read));

                        }
                    }
                    catch (Exception)
                    {
                        //System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;

                default:
                    try
                    {
                        if (DataTransfer.QueryQueue != null)
                        {
                            var temp =
                                DataTransfer.QueryQueue.ToArray()
                                    .Where(q => q.IsCycle == false)
                                    .Select(q => q)
                                    .ToArray();
                            DataTransfer.QueryQueue.Clear();
                            foreach (var t in temp)
                            {
                                DataTransfer.QueryQueue.Enqueue(t);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
        }


        /// <summary>
        /// Метод чтения/записи.
        /// Обрабатывает один объект, стоящий в очереди
        /// </summary>
        //private void ReadWriteData()
        //{
        //    if (DataTransfer.QueryQueue != null && DataTransfer.QueryQueue.Count != 0)
        //    {
        //        this.Dispatcher.Invoke(DispatcherPriority.SystemIdle,
        //            new Action(() => this.ReportUpdate(true, "Обновление данных")));
        //    }

        //    if (DataTransfer.QueryQueue != null && DataTransfer.QueryQueue.Count != 0)
        //    {
        //        var query = DataTransfer.QueryQueue.Dequeue();
        //        if (query.Operation == Accsess.Read)
        //        {
        //            query.Update();
        //        }
        //        else
        //        {
        //            query.WriteContext();
        //        }
        //        if (query.IsCycle)
        //        {
        //            DataTransfer.QueryQueue.Enqueue(query);
        //        }
        //    }
        //    Dispatcher.Invoke(DispatcherPriority.SystemIdle,
        //        new Action(() => this.ReportUpdate(false, "\")));
        //}


        public Luxmetr()
        {
            InitializeComponent();
            this.InitSlots();
            
        }

        private void InitSlots()
        {
           
        }

        private void ConnectionLost()
        {

        }
        private bool GetConnectionState()
        {
            return null != DataTransfer.ReadWords(_connectionChecker);
        }


        //private void ReportUpdate(bool value, string message)
        //{
        //    if (value)
        //    {
        //        this.uiStateIcon.Visibility = Visibility.Visible;
        //        this.uiStatePresenter.Text = message;
        //    }
        //    else
        //    {
        //        this.uiStateIcon.Visibility = Visibility.Hidden;
        //        this.uiStatePresenter.Text = message;
        //    }
        //}


     

        public void Start()
        {
            this._work = new Thread(this.DoWork) ;
        }



    }
}
