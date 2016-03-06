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
    NugetVersionPackages = fun config ->
      [ { NuGetPackage.Empty with
            FileName = "Yaaf.DependencyInjection.nuspec"
            ConfigFun = fun p ->
              { p with
                  ReleaseNotes = toLines release.Notes
                  Dependencies =
                    [ "FSharp.Core" ] 
                    |> List.map (fun name -> name, (GetPackageVersion "packages" name)) } }
        { NuGetPackage.Empty with 
            FileName = "Yaaf.DependencyInjection.Ninject.nuspec"
            Version = "1.1.0"
            SimpleName = "ninject"
            TagPrefix = "ninject_"
            ConfigFun = fun p ->
              { p with
                  ReleaseNotes = toLines release.Notes
                  Project = "Yaaf.DependencyInjection.Ninject"
                  Summary = "Yaaf.DependencyInjection.Ninject is a implementation of Yaaf.DependencyInjection for Ninject."
                  Description = "Yaaf.DependencyInjection.Ninject is a implementation of Yaaf.DependencyInjection for Ninject."
                  Dependencies = 
                    [ yield! [ "FSharp.Core"; "Portable.Ninject" ]
                        |> List.map (fun name -> name, (GetPackageVersion "packages" name))
                      yield config.ProjectName, config.Version |> RequireExactly ] } }
        { NuGetPackage.Empty with 
            FileName = "Yaaf.DependencyInjection.SimpleInjector.nuspec"
            Version = "1.2.0"
            SimpleName = "simpleinjector"
            TagPrefix = "simpleinjector_"
            ConfigFun = fun p ->
              { p with
                  ReleaseNotes = toLines release.Notes
                  Project = "Yaaf.DependencyInjection.SimpleInjector"
                  Summary = "Yaaf.DependencyInjection.SimpleInjector is a implementation of Yaaf.DependencyInjection for SimpleInjector."
                  Description = "Yaaf.DependencyInjection.SimpleInjector is a implementation of Yaaf.DependencyInjection for SimpleInjector."
                  Dependencies =
                    [ yield! [ "FSharp.Core"; "SimpleInjector" ] |> List.map (fun name -> name, (GetPackageVersion "packages" name))
                      yield config.ProjectName, config.Version |> RequireExactly ] } }
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
      let ninject = config.GetPackageByName "ninject"
      let ninjectSpec = ninject.ConfigFun (NuGetDefaults())
      let info =
        [ Attribute.Company ninjectSpec.Project
          Attribute.Product ninjectSpec.Project
          Attribute.Copyright config.CopyrightNotice
          Attribute.Version ninject.Version
          Attribute.FileVersion ninject.Version
          Attribute.InformationalVersion ninject.Version ]
      CreateCSharpAssemblyInfo "./src/SharedAssemblyInfo.Ninject.cs" info
      let simpleInjector = config.GetPackageByName "simpleinjector"
      let simpleInjectorSpec = ninject.ConfigFun (NuGetDefaults())
      let info =
        [ Attribute.Company simpleInjectorSpec.Project
          Attribute.Product simpleInjectorSpec.Project
          Attribute.Copyright config.CopyrightNotice
          Attribute.Version simpleInjector.Version
          Attribute.FileVersion simpleInjector.Version
          Attribute.InformationalVersion simpleInjector.Version ]
      CreateCSharpAssemblyInfo "./src/SharedAssemblyInfo.SimpleInjector.cs" info )
    RestrictReleaseToWindows = false
    DisableNUnit = true
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
          SimpleBuildName = "net40" }
       { BuildParams.WithSolution with
          // The default build
          PlatformName = "Profile111"
          SimpleBuildName = "profile111" }
       { BuildParams.WithSolution with
          // The default build
          PlatformName = "Net45"
          SimpleBuildName = "net45" } ]
  }
