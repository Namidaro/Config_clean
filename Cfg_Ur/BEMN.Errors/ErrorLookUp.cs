using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BEMN.Errors
{
    [Serializable]
    public class ErrorLookUp
    {
        private List<ErrorInfo> _errors = new List<ErrorInfo>();

        public List<ErrorInfo> Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }

        public static ErrorLookUp Open(string path)
        {
            Stream stream = null;
            if (!File.Exists(path))
                return null;
            try
            {
                stream = File.OpenRead(path);
                stream.Position = 0;
                var binSerializer = new BinaryFormatter();
                return (ErrorLookUp) binSerializer.Deserialize(stream);
            }
            catch (Exception)
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
            catch (Exception)
            {
                // игнорим ошибки или проверяем их
                result = false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return result;
        }
    }
}