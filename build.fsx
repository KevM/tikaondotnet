#r "packages/FAKE/tools/FakeLib.dll"
#r "System.IO.Compression.FileSystem.dll"
#r "packages/FSharp.Management/lib/net40/FSharp.Management.dll"

open Microsoft.FSharp.Core.Printf
open Fake
open Fake.AssemblyInfoFile
open Fake.Git
open Fake.ReleaseNotesHelper
open System
open System.IO
open FSharp.Management

let buildDir = "build"
let tempDir = "temp"
let tikaLibDir = "lib"

let solutionFile  = "src/TikaOnDotNet.sln"
let testAssemblies = "src/**/bin/Release/*Tests*.dll"

type root = FileSystem< __SOURCE_DIRECTORY__ >

let release =
  ReadFile "Release-Notes.md"
  |> ReleaseNotesHelper.parseReleaseNotes

// --------------------------------------------------------------------------------------
// IKVM.NET compilation helpers
let ikvmc = root.``paket-files``.``www.frijters.net``.``ikvm-8.1.5717.0``.bin.``ikvmc.exe``

type IKVMcTask(jar:string) =
  member val JarFile = jar
  member val Version = "" with get, set
  member val Dependencies = List.empty<IKVMcTask> with get, set

let timeOut = TimeSpan.FromSeconds(300.0)

let IKVMCompile workingDirectory tasks =
  let getNewFileName newExtension (fileName:string) =
      Path.GetFileName(fileName).Replace(Path.GetExtension(fileName), newExtension)
  let startProcess fileName args =
      let result =
          ExecProcess
              (fun info ->
                  info.FileName <- fileName
                  info.WorkingDirectory <- FullName workingDirectory
                  info.Arguments <- args)
              timeOut
      if result<> 0 then
          failwithf "Process '%s' failed with exit code '%d'" fileName result

  let rec compile (task:IKVMcTask) =
      let getIKVMCommandLineArgs() =
          let sb = Text.StringBuilder()
          task.Dependencies |> Seq.iter
              (fun x ->
                  compile x
                  x.JarFile |> getNewFileName ".dll" |> bprintf sb " -r:%s")
          if not <| String.IsNullOrEmpty(task.Version)
              then task.Version |> bprintf sb " -version:%s"
          bprintf sb " %s -out:%s"
              (task.JarFile |> getNewFileName ".jar")
              (task.JarFile |> getNewFileName ".dll")
          sb.ToString()

      File.Copy(task.JarFile, workingDirectory @@ (Path.GetFileName(task.JarFile)) ,true)
      startProcess ikvmc (getIKVMCommandLineArgs())
  tasks |> Seq.iter compile

Target "Clean" (fun _ ->
    CleanDirs [buildDir; tempDir; tikaLibDir]
)

Target "SetVersions" (fun _ ->
  let commitHash = Information.getCurrentSHA1 "."
  CreateCSharpAssemblyInfo "./SolutionInfo.cs"
        [Attribute.Version release.AssemblyVersion
         Attribute.FileVersion release.AssemblyVersion
         Attribute.Trademark commitHash]
)

Target "Build" (fun _ ->
    !! solutionFile
    |> MSBuildRelease "" "Clean;Rebuild"
    |> ignore
)

Target "RunTests" (fun _ ->
    !! testAssemblies
    |> NUnit (fun p ->
        { p with
            OutputFile = buildDir + "\TestResults.xml"})
)

type tikaDir = root.``paket-files``.``www-us.apache.org``

Target "CompileTikaLib" (fun _ ->
    [IKVMcTask(tikaDir.``tika-app-1.12.jar``, Version=release.AssemblyVersion, Dependencies = [])]
    |> IKVMCompile tikaLibDir
)

Target "PackageNugets" (fun _ ->
  Paket.Pack (fun p ->
        { p with
            Version = release.NugetVersion
            OutputPath = buildDir
            ReleaseNotes = toLines release.Notes
            Symbols = true })
)

"Clean"
  ==> "SetVersions"
  ==> "CompileTikaLib"
  ==> "Build"
  ==> "RunTests"
  ==> "PackageNugets"

// start build
RunTargetOrDefault "RunTests"
