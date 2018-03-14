/// Testing that the auto-opened name do not override any user-defined ones...
module SomeUserNamespace.SomeUserModule

module AsyncResult = 
  let defaultValue = ""

module Result = 
  let defaultValue = ""

let result = ""
let asyncResult = ""

let _ : string = Result.defaultValue
let _ : string = AsyncResult.defaultValue
let _ : string = result
let _ : string = asyncResult

/// ... but that the user may still use the library ones by opening them explicitly
open Cvdm.ErrorHandling

let _ : 'a -> Async<Result<'a, 'b>> -> Async<'a> = AsyncResult.defaultValue
let _ : 'a -> Result<'a, 'b> -> 'a = Result.defaultValue
let _ : ResultBuilder = result
let _ : AsyncResultBuilder = asyncResult
