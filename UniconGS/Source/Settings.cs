using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using UniconGS.UI.Configuration;
using UniconGS.UI.GPRS;
using UniconGS.UI.HeatingSchedule;
using UniconGS.UI.Schedule;

namespace UniconGS.Source
{
    [Serializable]
    public class Settings : IQuery
    {
        #region Delegate and events
        public delegate void ShowMessageEventHandler(string message, string caption);
        public event ShowMessageEventHandler ShowMessage;

        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();
        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;
        #endregion

        public Schedule LightSchedule { get; private set; }
        public Schedule BacklightSchedule { get; private set; }
        public Schedule IlluminationSchedule { get; private set; }
        public LogicConfig LogicConfig { get; private set; }
        public HeatingSchedule Heating { get; private set; }
        public GPRSConfiguration GPRS { get; private set; }

        public Settings(Schedule lightSchedule, Schedule backlightSchedule,
            Schedule illuminationSchedule, LogicConfig logicConfig, HeatingSchedule heating, GPRSConfiguration gprs)
        {
            this.LogicConfig = logicConfig;
            this.LightSchedule = lightSchedule;
            this.BacklightSchedule = backlightSchedule;
            this.IlluminationSchedule = illuminationSchedule;
            this.Heating = heating;
            this.GPRS = gprs;
        }

        public static Settings Open(string path)
        {
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = System.IO.File.OpenRead(path);
                stream.Position = 0;
                BinaryFormatter binSerializer = new BinaryFormatter();
                return (Settings)binSerializer.Deserialize(stream);
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (null != stream)
                {
                    stream.Flush();
                    stream.Close();
                }
            }
        }

        public bool Save(string path)
        {
            bool result;
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                result = true;
            }
            catch (Exception e)
            {
                // игнорим ошибки или проверяем их
                result = false;
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
            return result;
        }

        public void ApplySettings()
        {
            DataTransfer.SetTopInQueue(this, Accsess.Write, false);
        }

        #region IQueryMember
        public Slot Querer
        { get; set; }

        public ushort[] Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public bool WriteContext()
        {
            if (this.StartWork != null)
                this.StartWork();
            bool result = true;
            string message = string.Empty;
            if (DataTransfer.WriteWords(this.LogicConfig.Querer))
            {
                message += @"Конфигурация логики записана успешно.";
            }
            else
            {
                result = false;
                message += @"Ошибка записи конфигурации логики.";
            }
            if (DataTransfer.WriteWords(this.LightSchedule.Querer))
            {
                message += @"График освещения записан успешно;" + "\r\n";
            }
            else
            {
                result = false;
                message += @"Ошибка записи графика освещения;" + "\r\n"; 
            }
            if (DataTransfer.WriteWords(this.BacklightSchedule.Querer))
            {
                message += @"График подсветки записан успешно;" + "\r\n";
            }
            else
            {
                result = false;
                message += @"Ошибка записи графика подсветки;" + "\r\n";
            }
            if (DataTransfer.WriteWords(this.IlluminationSchedule.Querer))
            {
                message += @"График иллюминации записан успешно;" + "\r\n";
            }
            else
            {
                result = false;
                message += @"Ошибка записи графика иллюминации;" + "\r\n"; 
            }
            if (DataTransfer.WriteWords(this.Heating.Querer))
            {
                message += @"График обогрева записан успешно;" + "\r\n";
            }
            else
            {
                result = false;
                message += @"Ошибка записи графика обогрева;" + "\r\n"; 
            }
            if (DataTransfer.WriteWords(this.GPRS.Querer))
            {
                message += @"Конфигурация GPRS-модема записана успешно;" + "\r\n";
            }
            else
            {
                result = false;
                message += @"Ошибка записи конйигурации GPRS-модема;" + "\r\n";
            }
            if (this.StopWork != null)
                this.StopWork();
            if (result)
                ShowMessage(@"Применение настроек прошло успешно." + "\r\n" + message, "Применение настроек");
            else
                ShowMessage(@"Во время применения настроек произошла(и) ошибка(и)." + "\r\n" + message,
                    "Ошибка сохранения настроек");
            return result;
        }
        #endregion
    }
}
