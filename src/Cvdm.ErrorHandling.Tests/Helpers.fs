[<AutoOpen>]
module Cvdm.ErrorHandling.Tests.Helpers

/// Helper type for simulating side effects.
type Trigger() =
  let mutable isTriggered = false
  member __.Trigger () = isTriggered <- true
  member __.Triggered = isTriggered


/// Helper types for IDisposable tests
type CustomDisposedException() = 
  inherit exn()


type CustomDisposable() = 
  interface System.IDisposable with
    member __.Dispose() = 
      raise (CustomDisposedException())