using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniconGS.Source
{
    public interface IDataSource
    {
        byte[] ReadWrite(byte[] array);
        bool IsConnect { get; set; }
        bool Connect();
        bool Disconnect();
    }
}
