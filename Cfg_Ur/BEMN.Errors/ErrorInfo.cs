using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BEMN.Errors
{
    [Serializable]
    public class ErrorInfo
    {
        public DateTime Date { get; set; }
        public Exception ExceptionInfo { get; set; }
        public Assembly Assembly { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }

        public ErrorInfo(DateTime date, Exception exception,
            Assembly assembly, string author, string version)
        {
            this.Date = date;
            this.ExceptionInfo = exception;
            this.Assembly = assembly;
            this.Author = author;
            this.Version = version;
        }
    }
}
