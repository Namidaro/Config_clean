using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace UniconGS.Source
{
    [Serializable]
    public class Config
    {
        [NonSerialized]
        private static string _configPath = System.Windows.Forms.Application.StartupPath + @"\setting.cnfg";

        public string ScheduleInitialPath { get; set; }
        public string LogicConfigInitialPath { get; set; }
        public string AllSettingsImportInitialPath { get; set; }
        public string AllSettingsExportInitialPath { get; set; }

        public string AllSettingsImportInitialFilePath
        {
            get;
            set;
        }

        public string AllSettingsExportInitialFilePath
        {
            get;
            set;
        }

        public Config()
        {
            this.ScheduleInitialPath = string.Empty;
            this.LogicConfigInitialPath = string.Empty;
            this.AllSettingsExportInitialPath = string.Empty;
            this.AllSettingsImportInitialPath = string.Empty;
            this.AllSettingsExportInitialFilePath = string.Empty;
            this.AllSettingsImportInitialFilePath = string.Empty;
        }

        public static Config Open()
        {
            Stream stream = null;
            if (!File.Exists(_configPath))
                return null;
            try
            {

                IFormatter formatter = new BinaryFormatter();
                stream = System.IO.File.OpenRead(_configPath);
                stream.Position = 0;
                BinaryFormatter binSerializer = new BinaryFormatter();
                return (Config)binSerializer.Deserialize(stream);
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

        public void Save()
        {
            bool result = false;
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(_configPath, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                result = true;
            }
            catch (Exception e)
            {
                // игнорим ошибки или проверяем их
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
        }
    }
}
