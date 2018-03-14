/// Testing that the auto-opened name do not collide with user-defined ones
module SomeUserNamespace.SomeUserModule

module AsyncResult = 
  let map = ""

module Result = 
  let map = ""

let result = ""
let asyncResult = ""

let _ : string = AsyncResult.map
let _ : string = Result.map
let _ : string = result
let _ : string = asyncResult

