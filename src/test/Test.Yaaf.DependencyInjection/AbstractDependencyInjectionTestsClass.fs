namespace Test.Yaaf.DependencyInjection

open Yaaf.DependencyInjection
open NUnit.Framework
open Swensen.Unquote

type ITest =
    abstract Test : unit -> string
type Tester (data) =
    interface ITest with
        member x.Test () = data 

[<TestFixture>]
type NinjectTests() = 
    [<Test>]
    member x.``dummy to make nunit3 happy`` () =
        ()

[<AbstractClass>]
type AbstractDependencyInjectionTestsClass() = 
    
    abstract CreateKernel : unit -> IKernel
    
    [<Test>]
    member x.``check that we can modify the kernel after requesting a service`` () =
        let kernel = x.CreateKernel()
        kernel.Bind<ITest>().ToConstant(Tester("teststring"))
        let tester = kernel.Get<ITest>()
        test <@ tester.Test() = "teststring" @>
        kernel.Rebind<ITest>().ToConstant(Tester("secound"))
        let tester = kernel.Get<ITest>()
        test <@ tester.Test() = "secound" @>

        ()
        
    [<Test>]
    member x.``check that requesting IKernel works`` () =
        let kernel = x.CreateKernel()
        let service = kernel.Get<IKernel>()
        test <@ obj.ReferenceEquals(service, kernel) @>

        let child = kernel.CreateChild();
        let childService = child.Get<IKernel>()
        test <@ obj.ReferenceEquals(childService, child) @>
        ()
        
    [<Test>]
    member x.``check that child holds singleton`` () =
        let kernel = x.CreateKernel()
        kernel.Bind<ITest>().ToConstant(Tester("MyData"))
        
        let child = kernel.CreateChild();
        test <@ child.Get<ITest>().Test() = "MyData" @>
        ()
