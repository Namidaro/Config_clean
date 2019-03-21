using System.Collections.Generic;
using Unicon2.Infrastructure.DeviceInterfaces;
using Unicon2.Infrastructure.Interfaces;

namespace Unicon2.Infrastructure.FragmentInterfaces.FagmentSettings.QuickMemoryAccess
{
   public interface IQuickMemoryAccessDataProviderStub:IDataProvider,IDataProviderContaining
    {
        List<IMemoryValuesSet> MemoryValuesSets { get; set; }
    }
}
