using System;
using Microsoft.Practices.Unity;

namespace Unicon2.Infrastructure.Interceptors
{
    /// <summary>
    ///     Represents a wrapper for <see cref="IIoCContainer" /> that consumes <see cref="IUnityContainer" /> functionality
    /// </summary>
    public class UnityIoCWrapper : IIoCContainer
    {
        #region [Fields]

        private readonly IUnityContainer _container;

        #endregion [Fields]

        #region [Ctor's]

        /// <summary>
        ///     Creates an instance of <see cref="UnityIoCWrapper" />
        /// </summary>
        /// <param name="container">An instance of <see cref="IUnityContainer" /> to wrap</param>
        public UnityIoCWrapper(IUnityContainer container)
        {
            this._container = container;
        }

        #endregion [Ctor's]

        #region [IIoCContainer members]

        /// <summary>
        ///     Resolves a <see cref="Type" /> by the name of the mapping
        /// </summary>
        /// <typeparam name="T"><see cref="Type" /> to resolve</typeparam>
        /// <param name="mappingKey">The name of the mapping to resolve an instance with</param>
        /// <returns>An instance of retrieved object</returns>
        public T Resolve<T>(string mappingKey)
        {
            return this._container.Resolve<T>(mappingKey);
        }

        /// <summary>
        ///     Resolves a <see cref="Type" /> by the name of the mapping
        /// </summary>
        /// <typeparam name="T"><see cref="Type" /> to resolve</typeparam>
        /// <returns>An instance of the retrieved object</returns>
        public T Resolve<T>()
        {
            return this._container.Resolve<T>();
        }

        #endregion [IIoCContainer members]
    }
}