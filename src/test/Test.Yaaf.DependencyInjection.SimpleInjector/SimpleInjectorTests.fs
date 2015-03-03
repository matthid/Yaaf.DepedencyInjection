namespace Test.Yaaf.DependencyInjection.SimpleInjector
open Test.Yaaf.DependencyInjection
open Yaaf.DependencyInjection
open NUnit.Framework

[<TestFixture>]
type SimpleInjectorTests () = 
    inherit AbstractDependencyInjectionTestsClass()
    override x.CreateKernel() =
        SimpleInjectorKernelCreator.CreateKernel()
