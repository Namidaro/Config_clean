using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using UniconGS.UI.Configuration;
using UniconGS.UI.GPRS;

namespace UniconGS.UI.Settings
{
    [Serializable]
    public class Picon2Settings
    {
        public byte[] Picon2Config { get; private set; }
        public byte[] LightningSchedule { get; private set; }
        public byte[] IlluminationSchedule { get; private set; }
        public byte[] BacklightSchedule { get; private set; }


        public Picon2Settings(byte[] _picon2Config, byte[] _lightningSchedule, byte[] _illuminationSchedule, byte[] _backlightSchedule)
        {

            this.Picon2Config = _picon2Config;
            this.LightningSchedule = _lightningSchedule;
            this.IlluminationSchedule = _illuminationSchedule;
            this.BacklightSchedule = _backlightSchedule;

        }

        public static Picon2Settings Open(string path)
        {
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = System.IO.File.OpenRead(path);
                stream.Position = 0;

                BinaryFormatter binSerializer = new BinaryFormatter();

                return (Picon2Settings)binSerializer.Deserialize(stream);
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
            bool result = false;
            Stream stream = null;
            try
            {
                if (this.Picon2Config == null || this.LightningSchedule == null || this.IlluminationSchedule == null || this.BacklightSchedule == null)
                { throw new Exception(); }

                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                result = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Файл поврежден! Проверьте правильность данных конфигурации и графиков.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
