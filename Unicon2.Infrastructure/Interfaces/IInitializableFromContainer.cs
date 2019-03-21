using Microsoft.Practices.Unity;

namespace Unicon2.Infrastructure.Interfaces
{
    public interface IInitializableFromContainer
    {
        bool IsInitialized { get; }
        void InitializeFromContainer(IUnityContainer container);
    }
}