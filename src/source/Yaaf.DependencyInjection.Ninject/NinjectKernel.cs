// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using my = Yaaf.DependencyInjection;
using n = Ninject;

namespace Yaaf.DependencyInjection {
	public static class NinjectKernelCreator {
		public static IKernel CreateKernel ()
		{
			try {
				var kernel = new n.StandardKernel (Yaaf.DependencyInjection.Ninject.NinjectKernel.DefaultSettings);
				return CreateFromKernel(kernel);
			} catch (n.ActivationException err) {
				throw Yaaf.DependencyInjection.Ninject.NinjectKernel.WrapExn (err);
			}
		}
		public static IKernel CreateFromKernel (n.IKernel baseKernel)
		{
			if (baseKernel.GetBindings (typeof (my.IKernel)).Any ()) {
				throw new DependencyException ("Make sure that the given kernel doesn't bind to IKernel, because this bind is used internally");
			}
			return new Yaaf.DependencyInjection.Ninject.NinjectKernel (baseKernel);
		}
	}

}
namespace Yaaf.DependencyInjection.Ninject {
	using global::Ninject;
	using Microsoft.FSharp.Core;

    internal class NinjectKernel : my.IKernel
    {
		public static n.NinjectSettings DefaultSettings 
		{
			get {
				var settings = new n.NinjectSettings();
				settings.LoadExtensions = false;
				settings.ExtensionSearchPatterns = new string [] {};
				return settings;
			}
		}
		
		internal static DependencyException WrapExn(ActivationException err)
		{
 			return new DependencyException(err.Message, err);
		}

		internal static void ThrowExn (ActivationException err)
		{
			throw WrapExn(err);
		}

		private n.IKernel kernel;
		internal NinjectKernel (n.IKernel kernel)
		{
			this.kernel = kernel;
			try {
				this.kernel.Bind<my.IKernel> ().ToConstant (this);
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}

		private void CheckBind (Type serviceType)
		{
			if (serviceType == typeof (my.IKernel)) {
				throw new DependencyException ("Don't bind to IKernel, this slot is configured internally");
			}
		}

		public my.IBinder<TService> Bind<TService> ()
		{
			CheckBind (typeof(TService));
			try {
				return new NinjectBinder<TService> (kernel.Bind<TService> ());
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}


		public my.IBinder<object> Bind (Type serviceType)
		{
			CheckBind (serviceType);
			try {
				return new NinjectBinder<object> (kernel.Bind (serviceType));
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}


		public void Unbind<TService> ()
		{
			CheckBind (typeof (TService));
			try {
				kernel.Unbind<TService> ();
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}

		public void Unbind (Type serviceType)
		{
			CheckBind (serviceType);
			try {
				kernel.Unbind (serviceType);
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}

		public IBinder<TService> Rebind<TService> ()
		{
			CheckBind (typeof (TService));
			try {
				return new NinjectBinder<TService> (kernel.Rebind<TService> ());
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}

		public IBinder<object> Rebind (Type serviceType)
		{
			CheckBind (serviceType);
			try {
				return new NinjectBinder<object> (kernel.Rebind (serviceType));
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}
		public TService Get<TService> ()
		{
			try {
				return kernel.Get<TService> ();
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}

		public object Get (Type serviceType)
		{
			try {
				return kernel.Get (serviceType);
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}

		public my.IKernel CreateChild ()
		{
			try {
				return new NinjectKernel (
					new global::Ninject.Extensions.ChildKernel.ChildKernel (kernel, DefaultSettings));
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}

		public Microsoft.FSharp.Core.FSharpOption<TService> TryGet<TService> ()
		{
			try {
				var result = kernel.TryGet<TService> ();
				if (object.ReferenceEquals (result, null)) {
					return FSharpOption<TService>.None;
				} else {
					return FSharpOption<TService>.Some (result);
				}
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}

		public Microsoft.FSharp.Core.FSharpOption<object> TryGet (Type serviceType)
		{
			try {
				var result = kernel.TryGet (serviceType);
				if (object.ReferenceEquals (result, null)) {
					return FSharpOption<object>.None;
				} else {
					return FSharpOption<object>.Some (result);
				}
			} catch (ActivationException err) {
				throw WrapExn (err);
			}
		}
	}
}
