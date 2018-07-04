module Cvdm.ErrorHandling.Tests.HelperTests

open Xunit
open Hedgehog
open Swensen.Unquote

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
  let ``requireEqual returns ok if the values are equal`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<string>
      test <@ Result.requireEqual value value err = Ok () @>
    }


  [<Fact>]
  let ``requireEqual returns the specified error value if the values are not equal`` () =
    Property.check <| property {
      let! value1 = GenX.auto<string>
      let! value2 = GenX.auto<string> |> GenX.notEqualTo value1
      let! err = GenX.auto<string>
      test <@ Result.requireEqual value1 value2 err = Error err @>
    }


  [<Fact>]
  let ``requireEmpty returns ok if the sequence is empty`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ [] |> Seq.ofList |> Result.requireEmpty err = Ok () @>
    }


  [<Fact>]
  let ``requireEmpty returns the specified error value if the sequence is not empty`` () =
    Property.check <| property {
      let! nonEmptyList = GenX.auto<int> |> GenX.lList 1 10
      let! err = GenX.auto<string>
      test <@ nonEmptyList |> Seq.ofList |> Result.requireEmpty err = Error err @>
    }


  [<Fact>]
  let ``requireNotEmpty returns ok if the sequence is not empty`` () =
    Property.check <| property {
      let! nonEmptyList = GenX.auto<int> |> GenX.lList 1 10
      let! err = GenX.auto<string>
      test <@ nonEmptyList |> Seq.ofList |> Result.requireNotEmpty err = Ok () @>
    }


  [<Fact>]
  let ``requireNotEmpty returns the specified error value if the sequence is empty`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ [] |> Seq.ofList |> Result.requireNotEmpty err = Error err @>
    }


  [<Fact>]
  let ``requireHead returns the sequence head if the sequence is not empty`` () =
    Property.check <| property {
      let! nonEmptyList = GenX.auto<int> |> GenX.lList 1 10
      let! err = GenX.auto<string>
      test <@ nonEmptyList |> Seq.ofList |> Result.requireHead err = Ok nonEmptyList.Head @>
    }


  [<Fact>]
  let ``requireHead returns the specified error value if the sequence is empty`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ [] |> Seq.ofList |> Result.requireHead err = Error err @>
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


  let testAsync f x =
    async { return x } |> f |> Async.RunSynchronously


  [<Fact>]
  let ``map passes the ok value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Ok value
      |> testAsync (AsyncResult.map (fun x -> if x = value then t.Trigger()))
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``map does not call the function if error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Error value
      |> testAsync (AsyncResult.map (fun _ -> t.Trigger()))
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``map transforms the ok value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! mapRet = GenX.auto<int>
      test <@ Ok value |> testAsync (AsyncResult.map (fun _ -> mapRet)) = Ok mapRet @>
    }


  [<Fact>]
  let ``mapError passes the error value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Error value
      |> testAsync (AsyncResult.mapError (fun x -> if x = value then t.Trigger()))
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``mapError does not call the function if ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Ok value
      |> testAsync (AsyncResult.mapError (fun _ -> t.Trigger()))
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``mapError transforms the error value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! mapRet = GenX.auto<int>
      test <@ Error value |> testAsync (AsyncResult.mapError (fun _ -> mapRet)) = Error mapRet @>
    }


  [<Fact>]
  let ``requireTrue returns ok if true`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ true |> testAsync (AsyncResult.requireTrue err) = Ok () @>
    }


  [<Fact>]
  let ``requireTrue returns the specified error value if false`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ false |> testAsync (AsyncResult.requireTrue err) = Error err @>
    }


  [<Fact>]
  let ``requireFalse returns ok if false`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ false |> testAsync (AsyncResult.requireFalse err) = Ok () @>
    }


  [<Fact>]
  let ``requireFalse returns the specified error value if true`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ true |> testAsync (AsyncResult.requireFalse err) = Error err @>
    }


  [<Fact>]
  let ``requireSome returns the Some value if Some`` () =
    Property.check <| property {
      let! value = GenX.auto<int>
      let! err = GenX.auto<string>
      test <@ Some value |> testAsync (AsyncResult.requireSome err) = Ok value @>
    }


  [<Fact>]
  let ``requireSome returns the specified error value if None`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ None |> testAsync (AsyncResult.requireSome err) = Error err @>
    }


  [<Fact>]
  let ``requireNone returns ok if None`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ None |> testAsync (AsyncResult.requireNone err) = Ok () @>
    }


  [<Fact>]
  let ``requireNone returns the specified error value if Some`` () =
    Property.check <| property {
      let! value = GenX.auto<int>
      let! err = GenX.auto<string>
      test <@ Some value |> testAsync (AsyncResult.requireNone err) = Error err @>
    }


  [<Fact>]
  let ``requireEqualTo returns ok if the values are equal`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<string>
      test <@ value |> testAsync (AsyncResult.requireEqualTo value err) = Ok () @>
    }


  [<Fact>]
  let ``requireEqualTo returns the specified error value if the values are not equal`` () =
    Property.check <| property {
      let! value1 = GenX.auto<string>
      let! value2 = GenX.auto<string> |> GenX.notEqualTo value1
      let! err = GenX.auto<string>
      test <@ value1 |> testAsync (AsyncResult.requireEqualTo value2 err) = Error err @>
    }


  [<Fact>]
  let ``requireEmpty returns ok if the sequence is empty`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ [] |> Seq.ofList |> testAsync (AsyncResult.requireEmpty err) = Ok () @>
    }


  [<Fact>]
  let ``requireEmpty returns the specified error value if the sequence is not empty`` () =
    Property.check <| property {
      let! nonEmptyList = GenX.auto<int> |> GenX.lList 1 10
      let! err = GenX.auto<string>
      test <@ nonEmptyList |> Seq.ofList |> testAsync (AsyncResult.requireEmpty err) = Error err @>
    }


  [<Fact>]
  let ``requireNotEmpty returns ok if the sequence is not empty`` () =
    Property.check <| property {
      let! nonEmptyList = GenX.auto<int> |> GenX.lList 1 10
      let! err = GenX.auto<string>
      test <@ nonEmptyList |> Seq.ofList |> testAsync (AsyncResult.requireNotEmpty err) = Ok () @>
    }


  [<Fact>]
  let ``requireNotEmpty returns the specified error value if the sequence is empty`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ [] |> Seq.ofList |> testAsync (AsyncResult.requireNotEmpty err) = Error err @>
    }


  [<Fact>]
  let ``requireHead returns the sequence head if the sequence is not empty`` () =
    Property.check <| property {
      let! nonEmptyList = GenX.auto<int> |> GenX.lList 1 10
      let! err = GenX.auto<string>
      test <@ nonEmptyList |> Seq.ofList |> testAsync (AsyncResult.requireHead err) = Ok nonEmptyList.Head @>
    }


  [<Fact>]
  let ``requireHead returns the specified error value if the sequence is empty`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ [] |> Seq.ofList |> testAsync (AsyncResult.requireHead err) = Error err @>
    }


  [<Fact>]
  let ``withError replaces a unit error value with a custom error value`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      test <@ Error () |> testAsync (AsyncResult.withError err) = Error err @>
    }


  [<Fact>]
  let ``withError does not change an ok value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<int>
      test <@ Ok value |> testAsync (AsyncResult.withError err) = Ok value @>
    }


  [<Fact>]
  let ``setError replaces an error value with a custom error value`` () =
    Property.check <| property {
      let! errOriginal = GenX.auto<string>
      let! errNew = GenX.auto<int>
      test <@ Error errOriginal |> testAsync (AsyncResult.setError errNew) = Error errNew @>
    }


  [<Fact>]
  let ``setError does not change an ok value`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! err = GenX.auto<int>
      test <@ Ok value |> testAsync (AsyncResult.setError err) = Ok value @>
    }


  [<Fact>]
  let ``defaultValue returns the Ok value if Ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      test <@ Ok value |> testAsync (AsyncResult.defaultValue otherValue) = value @>
    }


  [<Fact>]
  let ``defaultValue returns the specified value if Error`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      test <@ Error err |> testAsync (AsyncResult.defaultValue otherValue) = otherValue @>
    }


  [<Fact>]
  let ``defaultWith returns the Ok value if Ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      test <@ Ok value |> testAsync (AsyncResult.defaultWith (fun () -> otherValue)) = value @>
    }


  [<Fact>]
  let ``defaultWith returns the specified value if Error`` () =
    Property.check <| property {
      let! err = GenX.auto<string>
      let! otherValue = GenX.auto<string>
      test <@ Error err |> testAsync (AsyncResult.defaultWith (fun () -> otherValue)) = otherValue @>
    }


  [<Fact>]
  let ``defaultWith does not evaluate the function if Ok`` () =
    Property.check <| property {
      let t = Trigger()
      let! value = GenX.auto<string>
      let! otherValue = GenX.auto<string>

      Ok value
      |> testAsync (AsyncResult.defaultWith (fun () -> t.Trigger(); otherValue))
      |> ignore

      test <@ not t.Triggered @>
    }


  // The compiler check this for us, but we have a test anyway so that
  // compilation errors from any erroneous changes are actually caught.
  [<Fact>]
  let ``ignoreError always returns unit`` () =
    Property.check <| property {
      let! result = GenX.auto<Result<unit,string>>
      test <@ result |> testAsync AsyncResult.ignoreError = () @>
    }


  [<Fact>]
  let ``teeIf runs the function if ok and the predicate returns true`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Ok value
      |> testAsync (AsyncResult.teeIf (fun _ -> true) (fun _ -> t.Trigger()))
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeIf does not run the function if not ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Error value
      |> testAsync (AsyncResult.teeIf (fun _ -> true) (fun _ -> t.Trigger()))
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeIf does not run the function if the predicate returns false`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Ok value
      |> testAsync (AsyncResult.teeIf (fun _ -> false) (fun _ -> t.Trigger()))
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeIf passes the ok value to the predicate`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      Ok value
      |> testAsync (AsyncResult.teeIf (fun x -> (if x = value then receivedCorrectValue.Trigger()); true) ignore)
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeIf passes the ok value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      Ok value
      |> testAsync (AsyncResult.teeIf (fun _ -> true) (fun x -> if x = value then receivedCorrectValue.Trigger()))
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeIf returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>
      test <@ value |> testAsync (AsyncResult.teeIf (fun _ -> true) ignore) = value @>
    }


  [<Fact>]
  let ``tee runs the function if ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Ok value
      |> testAsync (AsyncResult.tee (fun _ -> t.Trigger()))
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``tee does not run the function if not ok`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Error value
      |> testAsync (AsyncResult.tee (fun _ -> t.Trigger()))
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``tee passes the ok value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      Ok value
      |> testAsync (AsyncResult.tee (fun x -> if x = value then receivedCorrectValue.Trigger()))
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``tee returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>
      test <@ value |> testAsync (AsyncResult.tee ignore) = value @>
    }


  [<Fact>]
  let ``teeErrorIf runs the function if error and the predicate returns true`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Error value
      |> testAsync (AsyncResult.teeErrorIf (fun _ -> true) (fun _ -> t.Trigger()))
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf does not run the function if not error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Ok value
      |> testAsync (AsyncResult.teeErrorIf (fun _ -> true) (fun _ -> t.Trigger()))
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf does not run the function if the predicate returns false`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Error value
      |> testAsync (AsyncResult.teeErrorIf (fun _ -> false) (fun _ -> t.Trigger()))
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf passes the error value to the predicate`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      Error value
      |> testAsync (AsyncResult.teeErrorIf (fun x -> (if x = value then receivedCorrectValue.Trigger()); true) ignore)
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf passes the error value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      Error value
      |> testAsync (AsyncResult.teeErrorIf (fun _ -> true) (fun x -> if x = value then receivedCorrectValue.Trigger()))
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeErrorIf returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>
      test <@ value |> testAsync (AsyncResult.teeErrorIf (fun _ -> true) ignore) = value @>
    }


  [<Fact>]
  let ``teeError runs the function if error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Error value
      |> testAsync (AsyncResult.teeError (fun _ -> t.Trigger()))
      |> ignore

      test <@ t.Triggered @>
    }


  [<Fact>]
  let ``teeError does not run the function if not error`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let t = Trigger()

      Ok value
      |> testAsync (AsyncResult.teeError (fun _ -> t.Trigger()))
      |> ignore

      test <@ not t.Triggered @>
    }


  [<Fact>]
  let ``teeError passes the error value to the function`` () =
    Property.check <| property {
      let! value = GenX.auto<string>
      let receivedCorrectValue = Trigger()

      Error value
      |> testAsync (AsyncResult.teeError (fun x -> if x = value then receivedCorrectValue.Trigger()))
      |> ignore

      test <@ receivedCorrectValue.Triggered @>
    }


  [<Fact>]
  let ``teeError returns the original value`` () =
    Property.check <| property {
      let! value = GenX.auto<Result<string,int>>
      test <@ value |> testAsync (AsyncResult.teeError ignore) = value @>
    }
