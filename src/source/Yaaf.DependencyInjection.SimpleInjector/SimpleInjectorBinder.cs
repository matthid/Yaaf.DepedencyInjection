using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yaaf.DependencyInjection.SimpleInjector
{
    internal class SimpleInjectorBinder<TService> : IBinder<TService> where TService : class {

        private SimpleInjectorKernel kernel;
        private Type serviceType;
        private bool allowOverride;
        internal SimpleInjectorBinder(SimpleInjectorKernel kernel, Type serviceType = null, bool allowOverride = false)
        {
            this.kernel = kernel;
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

        internal Container Container { get { return kernel.Container; } set { kernel.Container = value; } }

        private void Secure(Action doSomething)
        {
            bool lockWasTaken = false;
            var temp = kernel.LockObj;
            var before = Container.Options.AllowOverridingRegistrations;
            try {
                Monitor.Enter(temp, ref lockWasTaken); 
                {
                    if (kernel.IsLocked)
                    {
                        Container = SimpleInjectorKernelCreator.Clone(Container);
                        kernel.IsLocked = false;
                    }
                    Container.Options.AllowOverridingRegistrations = allowOverride;
                    System.Diagnostics.Debug.Assert(!kernel.IsLocked);
                    doSomething(); 
                } 
            }
            finally {
                if (lockWasTaken) Monitor.Exit(temp);
                Container.Options.AllowOverridingRegistrations = before;
            }
        }

        public void To(Type instanceType)
        {
            this.Secure(() =>
            {
                Container.Register(serviceType ?? typeof(TService), instanceType);
            });
        }

        public void ToConstant<TInstance>(TInstance serviceInstance) where TInstance : TService
        {
            this.Secure(() =>
            {
                if (serviceType == null)
                {
                    Container.RegisterSingleton<TService>(serviceInstance);
                }
                else
                {
                    Container.RegisterSingleton(serviceType, serviceInstance);
                }
            });
        }

        public void To<TInstance>() where TInstance : class, TService
        {
            this.Secure(() =>
            {
                if (serviceType == null)
                {
                    Container.Register<TService, TInstance>();
                }
                else
                {
                    Container.Register(serviceType, typeof(TInstance));
                }
            });
        }
    }
}
