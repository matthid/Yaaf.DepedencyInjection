// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------

(*
    This file handles the configuration of the Yaaf.AdvancedBuilding build script.

    The first step is handled in build.sh and build.cmd by restoring either paket dependencies or bootstrapping a NuGet.exe and 
    executing NuGet to resolve all build dependencies (dependencies required for the build to work, for example FAKE).

    The secound step is executing build.fsx which loads this file (for configuration), builds the solution and executes all unit tests.
*)

#if FAKE
#else
// Support when file is opened in Visual Studio
#load "packages/Yaaf.AdvancedBuilding/content/buildConfigDef.fsx"
#endif

open BuildConfigDef
open System.Collections.Generic
open System.IO

open Fake
open Fake.Git
open Fake.FSharpFormatting
open AssemblyInfoFile

if isMono then
    monoArguments <- "--runtime=v4.0 --debug"

let buildConfig =
 // Read release notes document
 let release = ReleaseNotesHelper.parseReleaseNotes (File.ReadLines "doc/ReleaseNotes.md")
 let version_ninject = "1.1.0"
 let project_ninject = "Yaaf.DependencyInjection.Ninject"
 let version_simpleinjector = "1.1.1"
 let project_simpleinjector = "Yaaf.DependencyInjection.SimpleInjector"
 { BuildConfiguration.Defaults with
    ProjectName = "Yaaf.DependencyInjection"
    CopyrightNotice = "Yaaf.DependencyInjection Copyright © Matthias Dittrich 2015"
    ProjectSummary = "Yaaf.DependencyInjection is a simple abstraction layer over a (simple) dependency injection library."
    ProjectDescription = "Yaaf.DependencyInjection is a simple abstraction layer over a (simple) dependency injection library."
    ProjectAuthors = ["Matthias Dittrich"]
    NugetTags =  "dependency-injection C# F# dotnet .net ninject"
    PageAuthor = "Matthias Dittrich"
    GithubUser = "matthid"
    Version = release.NugetVersion
    NugetPackages =
      [ "Yaaf.DependencyInjection.nuspec", (fun config p ->
          { p with
              Version = config.Version
              ReleaseNotes = toLines release.Notes
              Dependencies = 
                [ "FSharp.Core", "3.1.2.1" ] })
        "Yaaf.DependencyInjection.Ninject.nuspec", (fun config p ->
          { p with
              Version = version_ninject
              ReleaseNotes = toLines release.Notes
              Project = project_ninject
              Summary = "Yaaf.DependencyInjection.Ninject is a implementation of Yaaf.DependencyInjection for Ninject."
              Description = "Yaaf.DependencyInjection.Ninject is a implementation of Yaaf.DependencyInjection for Ninject."
              Dependencies = 
                [ "FSharp.Core", "3.1.2.1"
                  "Portable.Ninject", "3.3.1"
                  config.ProjectName, config.Version ] })

        "Yaaf.DependencyInjection.SimpleInjector.nuspec", (fun config p ->
          { p with
              Version = version_simpleinjector
              ReleaseNotes = toLines release.Notes
              Project = project_simpleinjector
              Summary = "Yaaf.DependencyInjection.SimpleInjector is a implementation of Yaaf.DependencyInjection for SimpleInjector."
              Description = "Yaaf.DependencyInjection.SimpleInjector is a implementation of Yaaf.DependencyInjection for SimpleInjector."
              Dependencies =
                [ "FSharp.Core", "3.1.2.1"
                  "SimpleInjector", "2.7.2"
                  config.ProjectName, config.Version ] })
                   ]
    UseNuget = false
    SetAssemblyFileVersions = (fun config ->
      let info =
        [ Attribute.Company config.ProjectName
          Attribute.Product config.ProjectName
          Attribute.Copyright config.CopyrightNotice
          Attribute.Version config.Version
          Attribute.FileVersion config.Version
          Attribute.InformationalVersion config.Version ]
      CreateCSharpAssemblyInfo "./src/SharedAssemblyInfo.cs" info
      let info =
        [ Attribute.Company project_ninject
          Attribute.Product project_ninject
          Attribute.Copyright config.CopyrightNotice
          Attribute.Version version_ninject
          Attribute.FileVersion version_ninject
          Attribute.InformationalVersion version_ninject ]
      CreateCSharpAssemblyInfo "./src/SharedAssemblyInfo.Ninject.cs" info
      let info =
        [ Attribute.Company project_simpleinjector
          Attribute.Product project_simpleinjector
          Attribute.Copyright config.CopyrightNotice
          Attribute.Version version_simpleinjector
          Attribute.FileVersion version_simpleinjector
          Attribute.InformationalVersion version_simpleinjector ]
      CreateCSharpAssemblyInfo "./src/SharedAssemblyInfo.SimpleInjector.cs" info )
    EnableProjectFileCreation = false
    GeneratedFileList =
      [ "Yaaf.DependencyInjection.dll"
        "Yaaf.DependencyInjection.xml"
        "Yaaf.DependencyInjection.Ninject.dll"
        "Yaaf.DependencyInjection.Ninject.xml"
        "Yaaf.DependencyInjection.SimpleInjector.dll"
        "Yaaf.DependencyInjection.SimpleInjector.xml"  ]
    BuildTargets =
     [ { BuildParams.WithSolution with
          // The default build
          PlatformName = "Net40"
          // Workaround FSharp.Compiler.Service not liking to have a FSharp.Core here: https://github.com/fsprojects/FSharpx.Reflection/issues/1
          AfterBuild = fun _ -> File.Delete "build/net40/FSharp.Core.dll"
          SimpleBuildName = "net40" }
       { BuildParams.WithSolution with
          // The default build
          PlatformName = "Profile111"
          // Workaround FSharp.Compiler.Service not liking to have a FSharp.Core here: https://github.com/fsprojects/FSharpx.Reflection/issues/1
          AfterBuild = fun _ -> File.Delete "build/profile111/FSharp.Core.dll"
          SimpleBuildName = "profile111" }
       { BuildParams.WithSolution with
          // The default build
          PlatformName = "Net45"
          // Workaround FSharp.Compiler.Service not liking to have a FSharp.Core here: https://github.com/fsprojects/FSharpx.Reflection/issues/1
          AfterBuild = fun _ -> File.Delete "build/net45/FSharp.Core.dll"
          SimpleBuildName = "net45" } ]
  }
