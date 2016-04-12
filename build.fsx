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

MSBuildDefaults <- { MSBuildDefaults with Verbosity = Some MSBuildVerbosity.Minimal }

let artifactDir = "artifacts"
let tempDir = "temp"
let tikaLibDir = "lib"

let solutionFile  = "src/TikaOnDotNet.sln"
let [<Literal>]rootPath = __SOURCE_DIRECTORY__
let testAssemblies = "src/**/bin/Release/*Tests*.dll"
type root = FileSystem<rootPath>

let release =
  ReadFile "Release-Notes.md"
  |> ReleaseNotesHelper.parseReleaseNotes

// --------------------------------------------------------------------------------------
// IKVM.NET compilation helpers
let ikvmc = root.``paket-files``.``www.frijters.net``.``ikvm-8.1.5717.0``.bin.``ikvmc.exe``

type IKVMcTask(jar:string, assembly:string) =
  member val JarFile = jar
  member val AssemblyName = assembly
  member val Version = "" with get, set

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
          bprintf sb "-target:library -assembly:%s"
              (task.AssemblyName)
          if not <| String.IsNullOrEmpty(task.Version)
              then task.Version |> bprintf sb " -version:%s"
          bprintf sb " %s -out:%s"
              (task.JarFile |> getNewFileName ".jar")
              (task.AssemblyName + ".dll")
          sb.ToString()

      File.Copy(task.JarFile, workingDirectory @@ (Path.GetFileName(task.JarFile)) ,true)
      startProcess ikvmc (getIKVMCommandLineArgs())
  tasks |> Seq.iter compile

Target "Clean" (fun _ ->
    CleanDirs [artifactDir; tempDir; tikaLibDir]
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
            OutputFile = artifactDir + "\TestResults.xml"})
)

type tikaDir = root.``paket-files``.``www-us.apache.org``

Target "CompileTikaLib" (fun _ ->
    !! "paket-files/www-us.apache.org/tika-app-*.jar"
    |> Seq.map (fun name -> IKVMcTask(name, "TikaOnDotNet", Version=release.AssemblyVersion))
    |> IKVMCompile tikaLibDir
)

Target "PackageNugets" (fun _ ->
  Paket.Pack (fun p ->
        { p with
            Version = release.NugetVersion
            OutputPath = artifactDir
            ReleaseNotes = toLines release.Notes
            Symbols = true })
)

Target "PublishNugets" (fun _ ->
    Paket.Push(fun p ->
        { p with
            DegreeOfParallelism = 2
            WorkingDir = artifactDir })
)

"Clean"
  ==> "SetVersions"
  ==> "CompileTikaLib"
  ==> "Build"
  ==> "RunTests"
  ==> "PackageNugets"
  ==> "PublishNugets"

// start build
RunTargetOrDefault "RunTests"
