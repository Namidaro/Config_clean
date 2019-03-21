using System;
using System.Threading.Tasks;
using Unicon2.Infrastructure.Services.LogService;

namespace Unicon2.Infrastructure.DeviceInterfaces
{
    public interface IDeviceConnection :  IDisposable
    {
        string ConnectionName { get; }
        Task<bool> TryOpenConnectionAsync(bool isThrowingException,IComPortConfiguration comPort);
        Action<bool> LastQueryStatusChangedAction { get; set; }
        void CloseConnection();
    }
}