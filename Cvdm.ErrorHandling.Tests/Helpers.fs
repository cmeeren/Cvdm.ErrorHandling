[<AutoOpen>]
module Cvdm.ErrorHandling.Tests.Helpers

/// Helper type for simulating side effects.
type Trigger() =
  let mutable isTriggered = false
  member __.Trigger () = isTriggered <- true
  member __.Triggered = isTriggered
