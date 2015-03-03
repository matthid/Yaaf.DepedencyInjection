using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaaf.DependencyInjection
{
    public static class NinjectKernelCreator
    {
        public static IKernel CreateKernel()
        {
            try
            {
                var kernel = new Container(Yaaf.DependencyInjection.SimpleInjector.SimpleInjectorKernel.DefaultSettings);
                return CreateFromContainer(kernel);
            }
            catch (ActivationException err)
            {
                throw Yaaf.DependencyInjection.SimpleInjector.SimpleInjectorKernel.WrapExn(err);
            }
        }
        public static IKernel CreateFromContainer(Container baseKernel)
        {
            if (baseKernel.GetAllInstances<IKernel>().Any())
            {
                throw new DependencyException("Make sure that the given container doesn't bind to IKernel, because this bind is used internally");
            }
            return new Yaaf.DependencyInjection.SimpleInjector.SimpleInjectorKernel(baseKernel);
        }
    }

}
namespace Yaaf.DependencyInjection.SimpleInjector
{
    using Microsoft.FSharp.Core;

    internal class SimpleInjectorKernel : IKernel
    {
        public static ContainerOptions DefaultSettings
        {
            get
            {
                var settings = new ContainerOptions();
                return settings;
            }
        }

        internal static DependencyException WrapExn(ActivationException err)
        {
            return new DependencyException(err.Message, err);
        }

        internal static void ThrowExn(ActivationException err)
        {
            throw WrapExn(err);
        }

        private Container kernel;
        internal SimpleInjectorKernel(Container kernel)
        {
            this.kernel = kernel;
            try
            {
                this.kernel.RegisterSingle<IKernel>(this);
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }

        private void CheckBind(Type serviceType)
        {
            if (serviceType == typeof(IKernel))
            {
                throw new DependencyException("Don't bind to IKernel, this slot is configured internally");
            }
        }

        public IBinder<TService> Bind<TService>() where TService : class
        {
            CheckBind(typeof(TService));
            try
            {
                return new SimpleInjectorBinder<TService>(kernel);
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }


        public IBinder<object> Bind(Type serviceType)
        {
            CheckBind(serviceType);
            try
            {
                return new SimpleInjectorBinder<object>(kernel, serviceType);
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }


        public void Unbind<TService>() where TService : class
        {
            CheckBind(typeof(TService));
            try
            {
                throw new NotSupportedException("Unbind not supported by SimpleInjector"); ;
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }

        public void Unbind(Type serviceType)
        {
            CheckBind(serviceType);
            try
            {
                throw new NotSupportedException("Unbind not supported by SimpleInjector");
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }

        public IBinder<TService> Rebind<TService>() where TService : class
        {
            CheckBind(typeof(TService));
            try
            {
                return new SimpleInjectorBinder<TService>(kernel);
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }

        public IBinder<object> Rebind(Type serviceType)
        {
            CheckBind(serviceType);
            try
            {
                return new SimpleInjectorBinder<object>(kernel, serviceType);
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }
        public TService Get<TService>() where TService : class
        {
            try
            {
                return kernel.GetInstance<TService>();
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }

        public object Get(Type serviceType)
        {
            try
            {
                return kernel.GetInstance(serviceType);
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }

        public IKernel CreateChild()
        {
            try
            {
                var clone = new Container(DefaultSettings);
                foreach (var reg in kernel.GetCurrentRegistrations())
                {
                    clone.AddRegistration(reg.ServiceType, reg.Registration);
                }
                return new SimpleInjectorKernel(clone);
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }

        public Microsoft.FSharp.Core.FSharpOption<TService> TryGet<TService>() where TService : class
        {
            try
            {
                var result = kernel.GetAllInstances<TService>();
                if (result == null || !result.Any())
                {
                    return FSharpOption<TService>.None;
                }
                else
                {
                    return FSharpOption<TService>.Some(result.First());
                }
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }

        public Microsoft.FSharp.Core.FSharpOption<object> TryGet(Type serviceType)
        {
            try
            {
                var result = kernel.GetAllInstances(serviceType);
                if (result == null || !result.Any())
                {
                    return FSharpOption<object>.None;
                }
                else
                {
                    return FSharpOption<object>.Some(result.First());
                }
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }
    }
}
