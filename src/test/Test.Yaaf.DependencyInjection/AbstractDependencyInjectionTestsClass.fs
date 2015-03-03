namespace Test.Yaaf.DependencyInjection

open Yaaf.DependencyInjection
open NUnit.Framework
open Swensen.Unquote

type ITest =
    abstract Test : unit -> string
type Tester (data) =
    interface ITest with
        member x.Test () = data 

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
