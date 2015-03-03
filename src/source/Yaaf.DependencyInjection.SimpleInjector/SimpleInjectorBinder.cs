using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaaf.DependencyInjection.SimpleInjector
{
    public class SimpleInjectorBinder<TService> : IBinder<TService> where TService : class {

        private Container container;
        private Type serviceType;
        private bool allowOverride;
        public SimpleInjectorBinder(Container container, Type serviceType = null, bool allowOverride = false) { 
            this.container = container;
            this.serviceType = serviceType;
            this.allowOverride = allowOverride;
        }
        private class EndHelper : IDisposable
        {
            public Action doOnEnd;
            public EndHelper(Action doOnEnd)
            {
                this.doOnEnd = doOnEnd;
            }

            public void Dispose()
            {
                this.doOnEnd();
            }
        }
        private IDisposable Begin()
        {
            var before = container.Options.AllowOverridingRegistrations;
            container.Options.AllowOverridingRegistrations = allowOverride;
            return new EndHelper(() =>
                container.Options.AllowOverridingRegistrations = before);
        }
        public void To(Type instanceType)
        {
            using (this.Begin())
            {
                container.Register(serviceType ?? typeof(TService), instanceType);
            }
        }

        public void ToConstant<TInstance>(TInstance serviceInstance) where TInstance : TService
        {
            using (this.Begin())
            {
                if (serviceType == null)
                {
                    container.RegisterSingle<TService>(serviceInstance);
                }
                else
                {
                    container.RegisterSingle(serviceType, serviceInstance);
                }
            }
        }

        public void To<TInstance>() where TInstance : class, TService
        {
            using (this.Begin())
            {
                if (serviceType == null)
                {
                    container.Register<TService, TInstance>();
                }
                else
                {
                    container.Register(serviceType, typeof(TInstance));
                }
            }
        }
    }
}
