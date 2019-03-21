using ULA.Business.ConfigLogicalDevice;
using ULA.Business.Infrastructure.PersistanceServices;

namespace ULA.Devices.PICON2.Business
{
    public class Picon2ConfigLogicalDevice : ConfigLogicalDevice
    {
        public Picon2ConfigLogicalDevice(IPersistanceService persistanceService) : base(persistanceService)
        {
        }


        #region Overrides of ConfigLogicalDevice

   
        #endregion
    }
}
