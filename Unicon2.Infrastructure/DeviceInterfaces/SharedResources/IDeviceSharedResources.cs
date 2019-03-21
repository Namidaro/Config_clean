using System;
using System.Collections.Generic;
using Unicon2.Infrastructure.Interfaces;

namespace Unicon2.Infrastructure.DeviceInterfaces.SharedResources
{
    public interface IDeviceSharedResources:IDisposable
    {
        List<INameable> SharedResources { get; }
        void AddResource(INameable resource);
        void DeleteResource(INameable resource);
        bool IsItemReferenced(INameable nameable);
    }
}