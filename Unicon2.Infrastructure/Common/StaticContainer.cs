using Microsoft.Practices.Unity;

namespace Unicon2.Infrastructure.Common
{
    public static class StaticContainer
    {
        private static IUnityContainer _unityContainer;

        public static void SetContainer(IUnityContainer container)
        {
            _unityContainer = container;
        }

        public static IUnityContainer Container => _unityContainer;
    }
}
