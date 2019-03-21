
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UniconGS.UI.Configuration;
using UniconGS.UI.GPRS;

namespace UniconGS.UI.Settings
{
    [Serializable]
    public class Settings
    {
        public ushort[] LogicConfig { get; private set; }
        public ushort[] LightSchedule { get; private set; }
        public ushort[] BacklightSchedule { get; private set; }
        public ushort[] IlluminationSchedule { get; private set; }
        public ushort[] ConversationEnergy { get; private set; }
        public ushort[] Heating { get; private set; }
        public ushort[] GPRS { get; private set; }

        public Settings(ushort[] logicConfig, ushort[] lightSchedule, ushort[] backlightSchedule,
            ushort[] illuminationSchedule, ushort[] energy, ushort[] heating, ushort[] gprs)
        {
            this.LogicConfig = logicConfig;
            this.LightSchedule = lightSchedule;
            this.BacklightSchedule = backlightSchedule;
            this.IlluminationSchedule = illuminationSchedule;
            this.ConversationEnergy = energy;
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
            bool result = false;
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                result = true;
            }
            catch (Exception)
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
    }
}
