// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FSharp.Core;
using System.Collections.Concurrent;

namespace Yaaf.DependencyInjection
{ // Syntax similar to ninject
#if FULL_NET
	[Serializable]
#endif
	public class DependencyException : Exception {
		public DependencyException () { }
		public DependencyException (string message) : base (message) { }
		public DependencyException (string message, Exception inner) : base (message, inner) { }
#if FULL_NET
		protected DependencyException (
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base (info, context) { }
#endif
	}

	public interface IBinder<TService> {
		void To (Type instanceType);
		void ToConstant<TInstance> (TInstance serviceInstance) where TInstance : TService;
		void To<TInstance> () where TInstance : TService;
	}

    public interface IKernel {
		IBinder<TService> Bind<TService> ();
		IBinder<object> Bind (Type serviceType);

		void Unbind<TService> ();
		void Unbind (Type serviceType);

		IBinder<TService> Rebind<TService> ();
		IBinder<object> Rebind (Type serviceType);

		TService Get<TService> ();
		object Get (Type serviceType);


		FSharpOption<TService> TryGet<TService> ();
		FSharpOption<object> TryGet (Type serviceType);
		IKernel CreateChild ();
    }
}
