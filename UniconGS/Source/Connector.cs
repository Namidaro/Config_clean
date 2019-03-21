using System.IO.Ports;
using System.IO;
using System;
using System.Threading;
using System.Windows.Forms;
using NModbus4.Serial;

namespace UniconGS.Source
{
    public class Connector : IDataSource
    {
        public static SerialPort _port = null;
        private int _deviceNumber;
        private int _knNumber;
        private int _sleeper;
        private object _locker;
        //private AutoResetEvent _waiter;

        #region Properties
        public int DeviceNumber
        {
            get { return _deviceNumber; }
            set { _deviceNumber = value; }
        }

        public int KNNumber
        {
            get { return _knNumber; }
        }
        public bool IsConnect
        { get; set; }
        #endregion

        public Connector(string portName,int knNumber, int deviceNumber, int portSpeed, int timeOut)
        {
            _port = new SerialPort(portName, portSpeed, Parity.None, 8);
            _knNumber = 0; /*knNumber;*/
            _deviceNumber = deviceNumber;
            _port.ReadTimeout = timeOut;
            _port.WriteTimeout = timeOut;
            _sleeper = timeOut + 150;
            _locker = new object();

            //_port.DataReceived+= new SerialDataReceivedEventHandler(DataReceive);
            //_waiter = new AutoResetEvent(false);
        }

        public static SerialPortAdapter GetSerialPortAdapter(string portName, int deviceNumber, int portSpeed, int timeOut)
        {
         
           
            SerialPort serialPort = new SerialPort(portName, portSpeed, Parity.None, 8);

            serialPort.ReadTimeout = timeOut;
            serialPort.WriteTimeout = timeOut;

            SerialPortAdapter serialPortAdapter = new SerialPortAdapter(serialPort);
            serialPort.Open();
            return serialPortAdapter;
         
        }



        public void UnInit()
        {
            lock (_locker)
            {
                if (_port != null)
                {
                    Disconnect();
                    _port.Dispose();
                    _port = null;
                    _deviceNumber = 0;
                    _knNumber = 0;
                    _sleeper = 100;
                }
            }
        }

        public static string[] GetPorts()
        {
            return SerialPort.GetPortNames();
        }
        /// <summary>
        /// Открывает порт, выставляет IsConnect в true
        /// </summary>
        /// <returns>Возвращает состояние IsConnect</returns>
        public bool Connect()
        {
            lock (_locker)
            {


                if (_port.IsOpen) return true;





                if (_port.ReadTimeout == -1 || _port.WriteTimeout == -1)
                {
                    _port.ReadTimeout = 1000;
                    _port.WriteTimeout = 1000;
                }
                IsConnect = false;
                try
                {
                    _port.Open();
                    IsConnect = _port.IsOpen;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return IsConnect;
            }
        }
        /// <summary>
        /// Закрывает порт, меняет состояние IsConnect в false
        /// </summary>
        /// <returns>Возвращает состояние IsConnect</returns>
        public bool Disconnect()
        {
            lock (_locker)
            {
                try
                {
                    if (_port.IsOpen)
                    {
                        _port.Close();
                        IsConnect = _port.IsOpen;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return IsConnect;
            }
        }

        public byte[] ReadWrite(byte[] query)
        {
            lock (typeof(Connector))
            {
                if (_port != null)
                {
                    byte[] answer = new byte[256];

                    _port.Write(query, 0, query.Length);
                    //_waiter.WaitOne();
                    Thread.Sleep(_sleeper);
                    int count = _port.Read(answer, 0, answer.Length);
                    byte[] ret = new byte[count];
                    for (int i = 0; i < count; i++)
                    {
                        ret[i] = answer[i];
                    }
                    return ret;
                }
                else
                    throw new IOException("variable _port not initilized");
            }
        }

        /*Непонятно почему данные приходят более одного раза
        private void DataReceive(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(_sleeper);
            _waiter.Set();
        }*/
    }
}
