#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

Target.create "Clean" (fun _ ->
  !! "**/bin"
  ++ "**/obj"
  |> Shell.deleteDirs
)

Target.create "Build" (fun _ ->
  !! "src/**/*.*proj"
  |> Seq.iter (DotNet.build (fun x ->
      { x with
          Configuration = DotNet.BuildConfiguration.Release
      }
  ))
)

Target.create "Test" (fun _ ->
  "src/Cvdm.ErrorHandling.Tests"
  |> DotNet.test (fun x ->
      { x with
          Configuration = DotNet.BuildConfiguration.Release
      }
  )
)

Target.create "Pack" (fun _ ->
  "src/Cvdm.ErrorHandling"
  |> DotNet.pack (fun x ->
      { x with
          Configuration = DotNet.BuildConfiguration.Release
      }
  )
)

"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Pack"

Target.runOrDefault "Pack"
