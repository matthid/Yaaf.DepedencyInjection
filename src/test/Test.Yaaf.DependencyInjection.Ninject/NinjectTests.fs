namespace Test.Yaaf.DependencyInjection.Ninject

open Test.Yaaf.DependencyInjection
open Yaaf.DependencyInjection
open NUnit.Framework

[<TestFixture>]
type NinjectTests() = 
    inherit AbstractDependencyInjectionTestsClass()
    override x.CreateKernel() =
        NinjectKernelCreator.CreateKernel() 

