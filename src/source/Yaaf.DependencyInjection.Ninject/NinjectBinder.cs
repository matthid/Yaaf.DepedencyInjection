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

namespace Yaaf.DependencyInjection.Ninject {
	using global::Ninject;

	class NinjectBinder<TService> : my.IBinder<TService> {
		private n.Syntax.IBindingToSyntax<TService> binder;

		public NinjectBinder (n.Syntax.IBindingToSyntax<TService> binder)
		{
			this.binder = binder;
		}

		public void To<TInstance> () where TInstance : TService
		{
			try {
				binder.To<TInstance> ();
			} catch (ActivationException err) {
				throw NinjectKernel.WrapExn (err);
			}
		}

		public void To (Type instanceType)
		{
			try {
				binder.To (instanceType);
			} catch (ActivationException err) {
				throw NinjectKernel.WrapExn (err);
			}
		}

		public void ToConstant<TInstance> (TInstance serviceInstance) where TInstance : TService
		{
			try {
				binder.ToConstant<TInstance> (serviceInstance);
			} catch (ActivationException err) {
				throw NinjectKernel.WrapExn (err);
			}
		}
	}
}
