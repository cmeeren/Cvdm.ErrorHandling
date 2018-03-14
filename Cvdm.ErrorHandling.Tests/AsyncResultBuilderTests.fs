module Cvdm.ErrorHandling.Tests.AsyncResultBuilderTests

open System
open Xunit
open Hedgehog
open Swensen.Unquote
open Cvdm.ErrorHandling


[<Fact>]
let ``return wraps value in Ok`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result =
      asyncResult { return x }
      |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok x @>
  }


[<Fact>]
let ``return! returns async result value unmodified`` () =
  Property.check <| property {
    let! res = GenX.auto<Result<string, int>>
    let asyncRes = async { return res } |> AR

    let result =
      asyncResult { return! asyncRes }
      |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = res @>
  }

[<Fact>]
let ``return! returns result value wrapped in async`` () =
  Property.check <| property {
    let! res = GenX.auto<Result<string, int>>

    let result =
      asyncResult { return! res }
      |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = res @>
  }

[<Fact>]
let ``return! returns async wrapped in Ok`` () =
  Property.check <| property {
    let! res = GenX.auto<string>
    let asnc = async { return res }

    let result =
      asyncResult { return! asnc }
      |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok res @>
  }


[<Fact>]
let ``continues when do ok result`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result =
      asyncResult {
        do! Ok ()
        return x
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok x @>
  }


[<Fact>]
let ``continues when do async ok result`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result =
      asyncResult {
        do! async { return Ok () } |> AR
        return x
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok x @>
  }


[<Fact>]
let ``continues when do async simple value`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result =
      asyncResult {
        do! async { return () }
        return x
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok x @>
  }


[<Fact>]
let ``continues when let ok result`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result =
      asyncResult {
        let! y = Ok x
        return y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok x @>
  }


[<Fact>]
let ``continues when let async ok result`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result =
      asyncResult {
        let! y = async { return Ok x } |> AR
        return y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok x @>
  }


[<Fact>]
let ``continues when let async simple value`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result =
      asyncResult {
        let! y = async { return x }
        return y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok x @>
  }


[<Fact>]
let ``stops and returns error when do error result`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result =
      asyncResult {
        do! Error err
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }

[<Fact>]
let ``stops and returns error when do async error result`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result =
      asyncResult {
        do! async { return Error err } |> AR
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``stops and returns error when let error result`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result =
      asyncResult {
        let! x = Error err
        t.Trigger()
        return x
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``stops and returns error when let async error result`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result =
      asyncResult {
        let! x = async { return Error err } |> AR
        t.Trigger()
        return x
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }


let ``let! does not need type annotations when AsyncResult`` () =
  asyncResult {
    let! x = async { return Ok "" } |> AR
    return x.Length
  }


let ``let! does not need type annotations when async`` () =
  asyncResult {
    let! x = async { return "" }
    return x.Length
  }


let ``let! does not need type annotations when result`` () =
  asyncResult {
    let! x = Ok ""
    return x.Length
  }


[<Fact>]
let ``unwrapping and wrapping result gives original value`` () =
  Property.check <| property {
    let! res = GenX.auto<Result<string, int>>

    let result =
      asyncResult {
        let! y = res
        return y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = res @>
  }


[<Fact>]
let ``unwrapping and wrapping async result gives original value`` () =
  Property.check <| property {
    let! res = GenX.auto<Result<string, int>>
    let asyncRes = async { return res } |> AR

    let result =
      asyncResult {
        let! y = asyncRes
        return y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = res @>
  }


[<Fact>]
let ``wrapping and unwrapping gives original value`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result =
      asyncResult {
        let! y = asyncResult { return x }
        return y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok x @>
  }


[<Fact>]
let ``child workflow gives same result as inlined when initial wrapped value is result`` () =
  Property.check <| property {
    let! originalWrapped = GenX.auto<Result<string, int>>
    let! retFromF = GenX.auto<Result<char, int>>
    let! retFromG = GenX.auto<Result<bool, int>>
    let f _ = retFromF
    let g _ = retFromG

    let result1 =
      asyncResult {
        let! x = originalWrapped
        let! y = f x
        return! g y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    let result2 =
      asyncResult {
        let! y = asyncResult {
          let! x = originalWrapped
          return! f x
        }
        return! g y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result1 = result2 @>
  }

[<Fact>]
let ``child workflow gives same result as inlined when initial wrapped value is async-wrapped result`` () =
  Property.check <| property {
    let! originalWrapped = GenX.auto<Result<string, int>>
    let originalWrappedAsync = async { return originalWrapped }
    let! retFromF = GenX.auto<Result<char, int>>
    let! retFromG = GenX.auto<Result<bool, int>>
    let f _ = retFromF
    let g _ = retFromG

    let result1 =
      asyncResult {
        let! x = originalWrappedAsync
        let! y = f x
        return! g y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    let result2 =
      asyncResult {
        let! y = asyncResult {
          let! x = originalWrappedAsync
          return! f x
        }
        return! g y
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result1 = result2 @>
  }


[<Fact>]
let ``empty expression returns async-wrapped Ok ()`` () =
  Property.check <| property {
    let result = asyncResult { () } |> AsyncResult.toAsync |> Async.RunSynchronously
    test <@ result = Ok () @>
  }


[<Fact>]
let ``behavior of simple if when true`` () =
  Property.check <| property {
    let tIf = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then tIf.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of simple if when false`` () =
  Property.check <| property {
    let tIf = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if false then tIf.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ not tIf.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if with async-wrapped simple value`` () =
  Property.check <| property {
    let tIf = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! async { () }
          tIf.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if with ok value`` () =
  Property.check <| property {
    let tIf = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! Ok ()
          tIf.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if with async-wrapped ok value`` () =
  Property.check <| property {
    let tIf = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! async { return Ok () } |> AR
          tIf.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ t.Triggered @>
  }



[<Fact>]
let ``behavior of if with error value`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tIf = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! Error err
          tIf.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not tIf.Triggered @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``behavior of if with async-wrapped error value`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tIf = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! async { return Error err } |> AR
          tIf.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not tIf.Triggered @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``behavior of simple if-else when true`` () =
  Property.check <| property {
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then tIf.Trigger() else tElse.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ not tElse.Triggered @>
    test <@ t.Triggered @>
  }

[<Fact>]
let ``behavior of simple if-else when false`` () =
  Property.check <| property {
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if false then tIf.Trigger() else tElse.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ not tIf.Triggered @>
    test <@ tElse.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if-else with async-wrapped simple value`` () =
  Property.check <| property {
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! async { () }
          tIf.Trigger()
        else tElse.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ not tElse.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if-else with ok value`` () =
  Property.check <| property {
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! Ok ()
          tIf.Trigger()
        else tElse.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ not tElse.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if-else with async-wrapped ok value`` () =
  Property.check <| property {
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! async { return Ok () } |> AR
          tIf.Trigger()
        else tElse.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ not tElse.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if-else with error value`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! Error err
          tIf.Trigger()
        else tElse.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not tIf.Triggered @>
    test <@ not tElse.Triggered @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``behavior of if-else with async-wrapped error value`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result =
      asyncResult {
        if true then
          do! async { return Error err } |> AR
          tIf.Trigger()
        else tElse.Trigger()
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not tIf.Triggered @>
    test <@ not tElse.Triggered @>
    test <@ not t.Triggered @>
  }


// Tests for match expressions omitted, should work same as if-else


[<Fact>]
let ``behavior of simple for`` () =
  Property.check <| property {
    let ts = [Trigger(); Trigger(); Trigger()]

    let result =
      asyncResult {
        for t in ts do
          t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ ts |> List.forall (fun t -> t.Triggered) @>
  }


[<Fact>]
let ``behavior of for when some items fail, using result`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let ts =
      [(Trigger(), Ok (), Trigger())
       (Trigger(), Error err, Trigger())
       (Trigger(), Ok (), Trigger())]

    let result =
      asyncResult {
        for t1, res, t2 in ts do
          t1.Trigger()
          do! res
          t2.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    let (t1a, _, t1b) = ts.[0]
    let (t2a, _, t2b) = ts.[1]
    let (t3a, _, t3b) = ts.[2]

    test <@ result = Error err @>
    test <@ t1a.Triggered @>
    test <@ t1b.Triggered @>
    test <@ t2a.Triggered @>
    test <@ not t2b.Triggered @>
    test <@ not t3a.Triggered @>
    test <@ not t3b.Triggered @>
  }


[<Fact>]
let ``behavior of for when some items fail, using async-wrapped result`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let ts =
      [(Trigger(), Ok (), Trigger())
       (Trigger(), Error err, Trigger())
       (Trigger(), Ok (), Trigger())]

    let result =
      asyncResult {
        for t1, res, t2 in ts do
          t1.Trigger()
          do! async { return res } |> AR
          t2.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    let (t1a, _, t1b) = ts.[0]
    let (t2a, _, t2b) = ts.[1]
    let (t3a, _, t3b) = ts.[2]

    test <@ result = Error err @>
    test <@ t1a.Triggered @>
    test <@ t1b.Triggered @>
    test <@ t2a.Triggered @>
    test <@ not t2b.Triggered @>
    test <@ not t3a.Triggered @>
    test <@ not t3b.Triggered @>
  }


[<Fact>]
let ``behavior of simple while`` () =
  Property.check <| property {
    let ts = [Trigger(); Trigger(); Trigger()]
    use enum = (ts |> Seq.ofList).GetEnumerator()

    let result =
      asyncResult {
        while enum.MoveNext () do
          enum.Current.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ ts |> List.forall (fun t -> t.Triggered) @>
  }


[<Fact>]
let ``behavior of while when some items fail, using result`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let ts =
      [(Trigger(), Ok (), Trigger())
       (Trigger(), Error err, Trigger())
       (Trigger(), Ok (), Trigger())]
    use enum = (ts |> Seq.ofList).GetEnumerator()

    let result =
      asyncResult {
        while enum.MoveNext () do
          let t1, res, t2 = enum.Current
          t1.Trigger()
          do! res
          t2.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    let (t1a, _, t1b) = ts.[0]
    let (t2a, _, t2b) = ts.[1]
    let (t3a, _, t3b) = ts.[2]

    test <@ result = Error err @>
    test <@ t1a.Triggered @>
    test <@ t1b.Triggered @>
    test <@ t2a.Triggered @>
    test <@ not t2b.Triggered @>
    test <@ not t3a.Triggered @>
    test <@ not t3b.Triggered @>
  }


[<Fact>]
let ``behavior of while when some items fail, using async-wrapped result`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let ts =
      [(Trigger(), Ok (), Trigger())
       (Trigger(), Error err, Trigger())
       (Trigger(), Ok (), Trigger())]
    use enum = (ts |> Seq.ofList).GetEnumerator()

    let result =
      asyncResult {
        while enum.MoveNext () do
          let t1, res, t2 = enum.Current
          t1.Trigger()
          do! async { return res } |> AR
          t2.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    let (t1a, _, t1b) = ts.[0]
    let (t2a, _, t2b) = ts.[1]
    let (t3a, _, t3b) = ts.[2]

    test <@ result = Error err @>
    test <@ t1a.Triggered @>
    test <@ t1b.Triggered @>
    test <@ t2a.Triggered @>
    test <@ not t2b.Triggered @>
    test <@ not t3a.Triggered @>
    test <@ not t3b.Triggered @>
  }


[<Fact>]
let ``behavior of try-with-finally when not thrown`` () =
  Property.check <| property {
    let tTry = Trigger()
    let tCatch = Trigger()
    let tFinally = Trigger()

    let result =
      asyncResult {
        try
          try tTry.Trigger()
          with _ -> tCatch.Trigger()
        finally tFinally.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tTry.Triggered @>
    test <@ not tCatch.Triggered @>
    test <@ tFinally.Triggered @>
  }


[<Fact>]
let ``behavior of try-with-finally when thrown`` () =
  Property.check <| property {
    let tTry = Trigger()
    let tCatch = Trigger()
    let tFinally = Trigger()

    let result =
      asyncResult {
        try
          try
            failwith ""
            tTry.Trigger()
          with _ -> tCatch.Trigger()
        finally tFinally.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ not tTry.Triggered @>
    test <@ tCatch.Triggered @>
    test <@ tFinally.Triggered @>
  }


[<Fact>]
let ``simple use disposes`` () =
  Property.check <| property {
    let tDisp = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger()}

    let result =
      asyncResult {
        use _d = disp
        ()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``simple use disposes when doing async`` () =
  Property.check <| property {
    let tDisp = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger()}

    let result =
      asyncResult {
        use _d = disp
        do! async { () }
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``simple use disposes when result is ok`` () =
  Property.check <| property {
    let tDisp = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger()}

    let result =
      asyncResult {
        use _d = disp
        do! Ok ()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``simple use disposes when result is async-wrapped ok`` () =
  Property.check <| property {
    let tDisp = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger()}

    let result =
      asyncResult {
        use _d = disp
        do! async { return Ok () } |> AR
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``simple use disposes when result is error`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tDisp = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger() }

    let result =
      asyncResult {
        use _d = disp
        do! Error err
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``simple use disposes when result is async-wrapped error`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tDisp = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger() }

    let result =
      asyncResult {
        use _d = disp
        do! async { return Error err } |> AR
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``use! continues and disposes when disposable is ok`` () =
  Property.check <| property {
    let tDisp = Trigger()
    let t = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger()}

    let result =
      asyncResult {
        use! _d = Ok disp
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``use! continues and disposes when disposable is async-wrapped ok`` () =
  Property.check <| property {
    let tDisp = Trigger()
    let t = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger()}

    let result =
      asyncResult {
        use! _d = async { return Ok disp } |> AR
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``use! stops when disposable is error`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result =
      asyncResult {
        use! _d = Error err
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``use! stops when disposable is async-wrapped error`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result =
      asyncResult {
        use! _d = async { return Error err } |> AR
        t.Trigger()
      } |> AsyncResult.toAsync |> Async.RunSynchronously

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``use ignores null disposable`` () =
  Property.check <| property {
    let result =
      asyncResult {
        use _d = null
        do! Ok ()
      } |> AsyncResult.toAsync |> Async.RunSynchronously
    test <@ result = Ok () @>
  }


[<Fact>]
let ``use! ignores null Ok-wrapped disposable`` () =
  Property.check <| property {
    let result =
      asyncResult {
        use! _d = Ok null
        do! Ok ()
      } |> AsyncResult.toAsync |> Async.RunSynchronously
    test <@ result = Ok () @>
  }

[<Fact>]
let ``use! ignores null async and result wrapped disposable`` () =
  Property.check <| property {
    let result =
      asyncResult {
        use! _d = async { return Ok null } |> AR
        do! Ok ()
      } |> AsyncResult.toAsync |> Async.RunSynchronously
    test <@ result = Ok () @>
  }


[<Fact>]
let ``use! ignores null async-wrapped disposable`` () =
  Property.check <| property {
    let result =
      asyncResult {
        use! _d = async { return null }
        do! Ok ()
      } |> AsyncResult.toAsync |> Async.RunSynchronously
    test <@ result = Ok () @>
  }


type CustomDisposable() =
  member __.X = ""
  interface IDisposable with
    member __.Dispose () = ()


let ``use! is usable without type annotations when AsyncResult`` () =
  asyncResult {
    use! d = async { return Ok <| new CustomDisposable () } |> AR
    return d.X
  } |> AsyncResult.toAsync |> Async.RunSynchronously


let ``use! is usable without type annotations when async`` () =
  asyncResult {
    use! d = async { return new CustomDisposable () }
    return d.X
  } |> AsyncResult.toAsync |> Async.RunSynchronously


let ``use! is usable without type annotations when result`` () =
  asyncResult {
    use! d = Ok <| new CustomDisposable ()
    return d.X
  } |> AsyncResult.toAsync |> Async.RunSynchronously
