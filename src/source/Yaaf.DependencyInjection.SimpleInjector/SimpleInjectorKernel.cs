using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaaf.DependencyInjection
{
    public static class SimpleInjectorKernelCreator
    {
        public static IKernel CreateKernel()
        {
            try
            {
                var kernel = new Container();
                SimpleInjector.SimpleInjectorKernel.SetDefaultSettings(kernel.Options);
                return CreateFromContainer(kernel);
            }
            catch (ActivationException err)
            {
                throw Yaaf.DependencyInjection.SimpleInjector.SimpleInjectorKernel.WrapExn(err);
            }
        }
        public static IKernel CreateFromContainer(Container baseKernel)
        {
            if (baseKernel.GetRegistration(typeof(IKernel), false) != null)
            {
                throw new DependencyException("Make sure that the given container doesn't bind to IKernel, because this bind is used internally");
            }
            return new Yaaf.DependencyInjection.SimpleInjector.SimpleInjectorKernel(baseKernel);
        }
        internal static Container Clone(Container container)
        {
            var clone = new Container();
            SimpleInjector.SimpleInjectorKernel.SetDefaultSettings(clone.Options);
            clone.Options.AllowOverridingRegistrations = true;
            foreach (var reg in container.GetCurrentRegistrations())
            {
                clone.Register(reg.ServiceType, () => container.GetInstance(reg.ServiceType), reg.Lifestyle);                //clone.AddRegistration(reg.ServiceType, reg.Registration.);
            }
            clone.Options.AllowOverridingRegistrations = false;
            return clone;
        }
    }

}
namespace Yaaf.DependencyInjection.SimpleInjector
{
    using Microsoft.FSharp.Core;

    internal class SimpleInjectorKernel : IKernel
    {
        public static void SetDefaultSettings(ContainerOptions options)
        {
        }

        internal static DependencyException WrapExn(ActivationException err)
        {
            return new DependencyException(err.Message, err);
        }

        internal static void ThrowExn(ActivationException err)
        {
            throw WrapExn(err);
        }


        private object lockObj = new object();
        private bool isLocked = false;
        private Container kernel;
        internal SimpleInjectorKernel(Container kernel)
        {
            kernel = SimpleInjectorKernelCreator.Clone(kernel);
            this.kernel = kernel;
            var before = kernel.Options.AllowOverridingRegistrations;
            try
            {
                kernel.Options.AllowOverridingRegistrations = true;
                kernel.RegisterSingleton<IKernel>(this);
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
            finally
            {
                kernel.Options.AllowOverridingRegistrations = before;
            }
        }

        internal bool IsLocked { get { return isLocked; } set { isLocked = value; } }
        internal object LockObj { get { return lockObj; } }
        internal Container Container { get { return kernel; } set { kernel = value; } }
        
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
                return new SimpleInjectorBinder<TService>(this);
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
                return new SimpleInjectorBinder<object>(this, serviceType);
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
                return new SimpleInjectorBinder<TService>(this, allowOverride: true);
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
                return new SimpleInjectorBinder<object>(this, serviceType, allowOverride: true);
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
                lock (lockObj)
                {
                    isLocked = true;
                    return kernel.GetInstance<TService>();
                }
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
                lock (lockObj)
                {
                    isLocked = true;
                    return kernel.GetInstance(serviceType);
                }
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
                return new SimpleInjectorKernel(Container);
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
                lock (lockObj)
                {
                    isLocked = true;
                    var result = kernel.GetRegistration(typeof(TService), false);
                    if (result == null)
                    {
                        return FSharpOption<TService>.None;
                    }
                    else
                    {
                        return FSharpOption<TService>.Some(kernel.GetInstance<TService>());
                    }
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
                lock (lockObj)
                {
                    isLocked = true;
                    var result = kernel.GetRegistration(serviceType, false);
                    if (result == null)
                    {
                        return FSharpOption<object>.None;
                    }
                    else
                    {
                        return FSharpOption<object>.Some(kernel.GetInstance(serviceType));
                    }
                }
            }
            catch (ActivationException err)
            {
                throw WrapExn(err);
            }
        }
    }
}
