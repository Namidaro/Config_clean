using Prism.Mvvm;
using Unicon2.Connections.OfflineConnection.Interfaces;
using Unicon2.Infrastructure.DeviceInterfaces;
using Unicon2.Infrastructure.ViewModel;

namespace Unicon2.Connections.OfflineConnection.ViewModels
{
    public class OfflineConnectionViewModel : BindableBase, IOfflineConnectionViewModel
    {
        private IDeviceConnection _model;


        public OfflineConnectionViewModel()
        {
            DeviceConnection = new Connections.OfflineConnection.OfflineConnection();
        }

        public IDeviceConnection DeviceConnection { get; set; }

        public string StrongName =>nameof(OfflineConnectionViewModel);
        public object Model
        {
            get => DeviceConnection;
            set
            {
                if (value is OfflineConnection)
                    DeviceConnection = value as OfflineConnection;

            }
        }

        #region Implementation of IDeviceConnectionViewModel

        public string ConnectionName
        {
            get { return DeviceConnection.ConnectionName; }
        }

        #endregion

        #region Implementation of IViewModel<IDeviceConnection>

     

        #endregion
    }
}
