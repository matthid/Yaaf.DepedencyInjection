﻿using SimpleInjector;
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
            if (baseKernel.GetRegistration(typeof(IKernel), false) != null)
            {
                throw new DependencyException("Make sure that the given container doesn't bind to IKernel, because this bind is used internally");
            }
            return new Yaaf.DependencyInjection.SimpleInjector.SimpleInjectorKernel(Clone(baseKernel));
        }
        internal static Container Clone(Container container)
        {
            var clone = new Container(SimpleInjector.SimpleInjectorKernel.DefaultSettings);
            clone.Options.AllowOverridingRegistrations = true;
            foreach (var reg in container.GetCurrentRegistrations())
            {
                clone.Register(reg.ServiceType, reg.Registration.ImplementationType, reg.Lifestyle);
                //clone.AddRegistration(reg.ServiceType, reg.Registration.);
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


        private object lockObj = new object();
        private bool isLocked = false;
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
                return new SimpleInjectorKernel(SimpleInjectorKernelCreator.Clone(kernel));
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