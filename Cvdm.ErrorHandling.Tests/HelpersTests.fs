module Cvdm.ErrorHandling.Tests.HelperTests

open Xunit
open Hedgehog
open Swensen.Unquote
open Cvdm.ErrorHandling

module Result =


  [<Fact>]
  let ``requireTrue returns ok if true`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ true |> Result.requireTrue err = Ok () @>
    }


  [<Fact>]
  let ``requireTrue returns the specified error value if false`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ false |> Result.requireTrue err = Error err @>
    }


  [<Fact>]
  let ``requireFalse returns ok if false`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ false |> Result.requireFalse err = Ok () @>
    }


  [<Fact>]
  let ``requireFalse returns the specified error value if true`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ true |> Result.requireFalse err = Error err @>
    }


  [<Fact>]
  let ``requireSome returns the Some value if Some`` () =
    Property.check <| property {
      let! value = GenX.auto<int>
      let! err = GenX.auto<string>
      test <@ Some value |> Result.requireSome err = Ok value @>
    }


  [<Fact>]
  let ``requireSome returns the specified error value if None`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ None |> Result.requireSome err = Error err @>
    }


  [<Fact>]
  let ``requireNone returns ok if None`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ None |> Result.requireNone err = Ok () @>
    }


  [<Fact>]
  let ``requireNone returns the specified error value if Some`` () =
    Property.check <| property {
      let! value = GenX.auto<int>
      let! err = GenX.auto<string>
      test <@ Some value |> Result.requireNone err = Error err @>
    }


  [<Fact>]
  let ``requireEqualTo returns ok if the values are equal`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<string>
      test <@ value |> Result.requireEqualTo value err = Ok () @>
    }


  [<Fact>]
  let ``requireEqualTo returns the specified error value if the values are not equal`` () =
    Property.check <| property {
      let! value1 = GenX.auto<string>
      let! value2 = GenX.auto<string> |> GenX.notEqualTo value1
      let! err = GenX.auto<string>
      test <@ value1 |> Result.requireEqualTo value2 err = Error err @>
    }


  [<Fact>]
  let ``withError replaces a unit error value with a custom error value`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ Error () |> Result.withError err = Error err @>
    }


  [<Fact>]
  let ``withError does not change an ok value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<int>
      test <@ Ok value |> Result.withError err = Ok value @>
    }


  [<Fact>]
  let ``setError replaces an error value with a custom error value`` () =
    Property.check <| property {
      let! errOriginal = GenX.auto<string>
      let! errNew = GenX.auto<int>
      test <@ Error errOriginal |> Result.setError errNew = Error errNew @>
    }


  [<Fact>]
  let ``setError does not change an ok value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<int>
      test <@ Ok value |> Result.setError err = Ok value @>
    }


  [<Fact>]
  let ``defaultValue returns the Ok value if Ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      test <@ Ok value |> Result.defaultValue otherValue = value @>
    }


  [<Fact>]
  let ``defaultValue returns the specified value if Error`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      test <@ Error err |> Result.defaultValue otherValue = otherValue @>
    }


  [<Fact>]
  let ``defaultWith returns the Ok value if Ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      test <@ Ok value |> Result.defaultWith (fun () -> otherValue) = value @>
    }


  [<Fact>]
  let ``defaultWith returns the specified value if Error`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      test <@ Error err |> Result.defaultWith (fun () -> otherValue) = otherValue @>
    }


  [<Fact>]
  let ``defaultWith does not evaluate the function if Ok`` () =
    Property.check <| property {
      let t = Trigger()
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      Ok value |> Result.defaultWith (fun () -> t.Trigger(); otherValue) |> ignore
      test <@ not t.Triggered @>
    }


  // The compiler check this for us, but we have a test anyway so that
  // compilation errors from any erroneous changes are actually caught.
  [<Fact>]
  let ``ignoreError returns the Ok value if Ok`` () =
    Property.check <| property {
      let! result = GenX.auto<Result<unit,string>>
      test <@ result |> Result.ignoreError = () @>
    }


  [<Fact>]
  let ``teeIf runs the function if ok and the predicate returns true`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Ok value |> Result.teeIf (fun _ -> true) (fun _ -> t.Trigger()) |> ignore
      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeIf does not run the function if not ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Error value |> Result.teeIf (fun _ -> true) (fun _ -> t.Trigger()) |> ignore
      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeIf does not run the function if the predicate returns false`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Ok value |> Result.teeIf (fun _ -> false) (fun _ -> t.Trigger()) |> ignore
      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeIf passes the ok value to the predicate`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()
      Ok value |> Result.teeIf (fun x -> (if x = value then receivedCorrectValue.Trigger()); true) ignore |> ignore
      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeIf passes the ok value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()
      Ok value |> Result.teeIf (fun _ -> true) (fun x -> if x = value then receivedCorrectValue.Trigger()) |> ignore
      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeIf returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>
      test <@ value |> Result.teeIf (fun _ -> true) ignore = value @>
    }


  [<Fact>]
  let ``tee runs the function if ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Ok value |> Result.tee (fun _ -> t.Trigger()) |> ignore
      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``tee does not run the function if not ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Error value |> Result.tee (fun _ -> t.Trigger()) |> ignore
      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``tee passes the ok value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()
      Ok value |> Result.tee (fun x -> if x = value then receivedCorrectValue.Trigger()) |> ignore
      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``tee returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>
      test <@ value |> Result.tee ignore = value @>
    }



  [<Fact>]
  let ``teeErrorIf runs the function if error and the predicate returns true`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Error value |> Result.teeErrorIf (fun _ -> true) (fun _ -> t.Trigger()) |> ignore
      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf does not run the function if not error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Ok value |> Result.teeErrorIf (fun _ -> true) (fun _ -> t.Trigger()) |> ignore
      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf does not run the function if the predicate returns false`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Error value |> Result.teeErrorIf (fun _ -> false) (fun _ -> t.Trigger()) |> ignore
      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf passes the error value to the predicate`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()
      Error value |> Result.teeErrorIf (fun x -> (if x = value then receivedCorrectValue.Trigger()); true) ignore |> ignore
      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf passes the error value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()
      Error value |> Result.teeErrorIf (fun _ -> true) (fun x -> if x = value then receivedCorrectValue.Trigger()) |> ignore
      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>
      test <@ value |> Result.teeErrorIf (fun _ -> true) ignore = value @>
    }


  [<Fact>]
  let ``teeError runs the function if error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Error value |> Result.teeError (fun _ -> t.Trigger()) |> ignore
      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeError does not run the function if not error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()
      Ok value |> Result.teeError (fun _ -> t.Trigger()) |> ignore
      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeError passes the error value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()
      Error value |> Result.teeError (fun x -> if x = value then receivedCorrectValue.Trigger()) |> ignore
      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeError returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>
      test <@ value |> Result.teeError ignore = value @>
    }



module AsyncResult =


  [<Fact>]
  let ``map passes the ok value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Ok value }
      |> AsyncResult.map (fun x -> if x = value then t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``map does not call the function if error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Error value }
      |> AsyncResult.map (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``map transforms the ok value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! mapRet = GenX.auto<int>

      let res =
        async { return Ok value }
        |> AsyncResult.map (fun _ -> mapRet)
        |> Async.RunSynchronously

      test <@ res = Ok mapRet @>
    }


  [<Fact>]
  let ``mapError passes the error value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Error value }
      |> AsyncResult.mapError (fun x -> if x = value then t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``mapError does not call the function if ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Ok value }
      |> AsyncResult.mapError (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``mapError transforms the error value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! mapRet = GenX.auto<int>

      let res =
        async { return Error value }
        |> AsyncResult.mapError (fun _ -> mapRet)
        |> Async.RunSynchronously

      test <@ res = Error mapRet @>
    }


  [<Fact>]
  let ``requireTrue returns ok if true`` () =
    Property.check <| property {
      let! err = GenX.auto<string>

      let result =
        async { return true }
        |> AsyncResult.requireTrue err
        |> Async.RunSynchronously

      test <@ result = Ok () @>
    }


  [<Fact>]
  let ``requireTrue returns the specified error value if false`` () =
    Property.check <| property {
      let! err = GenX.auto<string>

      let result =
        async { return false }
        |> AsyncResult.requireTrue err
        |> Async.RunSynchronously

      test <@ result = Error err @>
    }


  [<Fact>]
  let ``requireFalse returns ok if false`` () =
    Property.check <| property {
      let! err = GenX.auto<string>

      let result =
        async { return false }
        |> AsyncResult.requireFalse err
        |> Async.RunSynchronously

      test <@ result = Ok () @>
    }


  [<Fact>]
  let ``requireFalse returns the specified error value if true`` () =
    Property.check <| property {
      let! err = GenX.auto<string>

      let result =
        async { return true }
        |> AsyncResult.requireFalse err
        |> Async.RunSynchronously

      test <@ result = Error err @>
    }


  [<Fact>]
  let ``requireSome returns the Some value if Some`` () =
    Property.check <| property {
      let! value = GenX.auto<int>
      let! err = GenX.auto<string>

      let result =
        async { return Some value }
        |> AsyncResult.requireSome err
        |> Async.RunSynchronously

      test <@ result = Ok value @>
    }


  [<Fact>]
  let ``requireSome returns the specified error value if None`` () =
    Property.check <| property {
      let! err = GenX.auto<string>

      let result =
        async { return None }
        |> AsyncResult.requireSome err
        |> Async.RunSynchronously

      test <@ result = Error err @>
    }


  [<Fact>]
  let ``requireNone returns ok if None`` () =
    Property.check <| property {
      let! err = GenX.auto<string>

      let result =
        async { return None }
        |> AsyncResult.requireNone err
        |> Async.RunSynchronously

      test <@ result = Ok () @>
    }


  [<Fact>]
  let ``requireNone returns the specified error value if Some`` () =
    Property.check <| property {
      let! value = GenX.auto<int>
      let! err = GenX.auto<string>

      let result =
        async { return Some value }
        |> AsyncResult.requireNone err
        |> Async.RunSynchronously

      test <@ result = Error err @>
    }


  [<Fact>]
  let ``withError replaces a unit error value with a custom error value`` () =
    Property.check <| property {
      let! err = GenX.auto<string>

      let res =
        async { return Error () }
        |> AsyncResult.withError err
        |> Async.RunSynchronously

      test <@ res = Error err @>
    }


  [<Fact>]
  let ``withError does not change an ok value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<int>

      let res =
        async { return Ok value }
        |> AsyncResult.withError err
        |> Async.RunSynchronously

      test <@ res = Ok value @>
    }


  [<Fact>]
  let ``setError replaces an error value with a custom error value`` () =
    Property.check <| property {
      let! errOriginal = GenX.auto<string>
      let! errNew = GenX.auto<int>

      let res =
        async { return Error errOriginal }
        |> AsyncResult.setError errNew
        |> Async.RunSynchronously

      test <@ res = Error errNew @>
    }


  [<Fact>]
  let ``setError does not change an ok value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<int>

      let res =
        async { return Ok value }
        |> AsyncResult.setError err
        |> Async.RunSynchronously

      test <@ res = Ok value @>
    }


  [<Fact>]
  let ``defaultValue returns the Ok value if Ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>

      let res =
        async { return Ok value }
        |> AsyncResult.defaultValue otherValue
        |> Async.RunSynchronously

      test <@ res = value @>
    }


  [<Fact>]
  let ``defaultValue returns the specified value if Error`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      let! otherValue = GenX.auto<string>

      let res =
        async { return Error err }
        |> AsyncResult.defaultValue otherValue
        |> Async.RunSynchronously

      test <@ res = otherValue @>
    }


  [<Fact>]
  let ``defaultWith returns the Ok value if Ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>

      let res =
        async { return Ok value }
        |> AsyncResult.defaultWith (fun () -> otherValue)
        |> Async.RunSynchronously

      test <@ res = value @>
    }


  [<Fact>]
  let ``defaultWith returns the specified value if Error`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      let! otherValue = GenX.auto<string>

      let res =
        async { return Error err }
        |> AsyncResult.defaultWith (fun () -> otherValue)
        |> Async.RunSynchronously

      test <@ res = otherValue @>
    }


  [<Fact>]
  let ``defaultWith does not evaluate the function if Ok`` () =
    Property.check <| property {
      let t = Trigger()
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>

      async { return Ok value }
      |> AsyncResult.defaultWith (fun () -> t.Trigger(); otherValue)
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  // The compiler check this for us, but we have a test anyway so that
  // compilation errors from any erroneous changes are actually caught.
  [<Fact>]
  let ``ignoreError always returns unit`` () =
    Property.check <| property {
      let! result = GenX.auto<Result<unit,string>>
      let res =
        async { return result }
        |> AsyncResult.ignoreError
        |> Async.RunSynchronously

      test <@ res = () @>
    }


  [<Fact>]
  let ``teeIf runs the function if ok and the predicate returns true`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Ok value }
      |> AsyncResult.teeIf (fun _ -> true) (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeIf does not run the function if not ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Error value }
      |> AsyncResult.teeIf (fun _ -> true) (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeIf does not run the function if the predicate returns false`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Ok value }
      |> AsyncResult.teeIf (fun _ -> false) (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeIf passes the ok value to the predicate`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      async { return Ok value }
      |> AsyncResult.teeIf (fun x -> (if x = value then receivedCorrectValue.Trigger()); true) ignore
      |> Async.RunSynchronously
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeIf passes the ok value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      async { return Ok value }
      |> AsyncResult.teeIf (fun _ -> true) (fun x -> if x = value then receivedCorrectValue.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeIf returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>

      let res =
        async { return value }
        |> AsyncResult.teeIf (fun _ -> true) ignore
        |> Async.RunSynchronously

      test <@ res = value @>
    }


  [<Fact>]
  let ``tee runs the function if ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Ok value }
      |> AsyncResult.tee (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``tee does not run the function if not ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Error value }
      |> AsyncResult.tee (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``tee passes the ok value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      async { return Ok value }
      |> AsyncResult.tee (fun x -> if x = value then receivedCorrectValue.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``tee returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>

      let res =
        async { return value }
        |> AsyncResult.tee ignore
        |> Async.RunSynchronously

      test <@ res = value @>
    }


  [<Fact>]
  let ``teeErrorIf runs the function if error and the predicate returns true`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Error value }
      |> AsyncResult.teeErrorIf (fun _ -> true) (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf does not run the function if not error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Ok value }
      |> AsyncResult.teeErrorIf (fun _ -> true) (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf does not run the function if the predicate returns false`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Error value }
      |> AsyncResult.teeErrorIf (fun _ -> false) (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf passes the error value to the predicate`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      async { return Error value }
      |> AsyncResult.teeErrorIf (fun x -> (if x = value then receivedCorrectValue.Trigger()); true) ignore
      |> Async.RunSynchronously
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf passes the error value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      async { return Error value }
      |> AsyncResult.teeErrorIf (fun _ -> true) (fun x -> if x = value then receivedCorrectValue.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>

      let res =
        async { return value }
        |> AsyncResult.teeErrorIf (fun _ -> true) ignore
        |> Async.RunSynchronously

      test <@ res = value @>
    }


  [<Fact>]
  let ``teeError runs the function if error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Error value }
      |> AsyncResult.teeError (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeError does not run the function if not error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      async { return Ok value }
      |> AsyncResult.teeError (fun _ -> t.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeError passes the error value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      async { return Error value }
      |> AsyncResult.teeError (fun x -> if x = value then receivedCorrectValue.Trigger())
      |> Async.RunSynchronously
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeError returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>

      let res =
        async { return value }
        |> AsyncResult.teeError ignore
        |> Async.RunSynchronously

      test <@ res = value @>
    }
