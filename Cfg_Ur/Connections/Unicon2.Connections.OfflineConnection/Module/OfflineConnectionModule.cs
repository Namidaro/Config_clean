using Prism.Modularity;
using Unicon2.Infrastructure.Services;

namespace Unicon2.Connections.OfflineConnection.Module
{
   public class OfflineConnectionModule:IModule
    {
        private readonly ISerializerService _serializerService;


        public OfflineConnectionModule(ISerializerService serializerService)
        {
            _serializerService = serializerService;
        }
        #region Implementation of IModule

        public void Initialize()
        {
            _serializerService.AddKnownTypeForSerialization(typeof(OfflineConnection));
        }

        #endregion
    }
}
