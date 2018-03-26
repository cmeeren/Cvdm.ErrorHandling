module Cvdm.ErrorHandling.Tests.ResultBuilderTests

open System
open Xunit
open Hedgehog
open Swensen.Unquote

[<Fact>]
let ``return wraps value in Ok`` () =
  Property.check <| property {
    let! initialUnwrapped = GenX.auto<string>
    let result = result { return initialUnwrapped }
    test <@ result = Ok initialUnwrapped @>
  }


[<Fact>]
let ``return! returns value unmodified`` () =
  Property.check <| property {
    let! initialWrapped = GenX.auto<Result<string, int>>
    let result = result { return! initialWrapped }
    test <@ result = initialWrapped @>
  }


[<Fact>]
let ``continues when do ok`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result = result {
      do! Ok ()
      return x
    }

    test <@ result = Ok x @>
  }


[<Fact>]
let ``continues when let ok`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result = result {
      let! y = Ok x
      return y
    }

    test <@ result = Ok x @>
  }


[<Fact>]
let ``stops and returns error when do error`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result = result {
      do! Error err
      t.Trigger()
    }

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``stops and returns error when let error`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result = result {
      let! x = Error err
      t.Trigger()
      return x
    }

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``unwrapping and wrapping gives original value`` () =
  Property.check <| property {
    let! x = GenX.auto<Result<string, int>>

    let result = result {
      let! y = x
      return y
    }

    test <@ result = x @>
  }


[<Fact>]
let ``wrapping and unwrapping gives original value`` () =
  Property.check <| property {
    let! x = GenX.auto<string>

    let result = result {
      let! y = result { return x }
      return y
    }

    test <@ result = Ok x @>
  }


[<Fact>]
let ``child workflow gives same result as inlined`` () =
  Property.check <| property {
    let! originalWrapped = GenX.auto<Result<string, int>>
    let! retFromF = GenX.auto<Result<char, int>>
    let! retFromG = GenX.auto<Result<bool, int>>
    let f _ = retFromF
    let g _ = retFromG

    let result1 = result {
      let! x = originalWrapped
      let! y = f x
      return! g y
    }

    let result2 = result {
      let! y = result {
        let! x = originalWrapped
        return! f x
      }
      return! g y
    }

    test <@ result1 = result2 @>
  }


[<Fact>]
let ``empty expression returns Ok ()`` () =
  Property.check <| property {
    let result = result { () }
    test <@ result = Ok () @>
  }


[<Fact>]
let ``behavior of simple if when true`` () =
  Property.check <| property {
    let tIf = Trigger()
    let t = Trigger()

    let result = result {
      if true then tIf.Trigger()
      t.Trigger()
    }

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of simple if when false`` () =
  Property.check <| property {
    let tIf = Trigger()
    let t = Trigger()

    let result = result {
      if false then tIf.Trigger()
      t.Trigger()
    }

    test <@ result = Ok () @>
    test <@ not tIf.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if with wrapped ok value`` () =
  Property.check <| property {
    let tIf = Trigger()
    let t = Trigger()

    let result = result {
      if true then
        do! Ok ()
        tIf.Trigger()
      t.Trigger()
    }

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ t.Triggered @>
  }



[<Fact>]
let ``behavior of if with wrapped error value`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tIf = Trigger()
    let t = Trigger()

    let result = result {
      if true then
        do! Error err
        tIf.Trigger()
      t.Trigger()
    }

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

    let result = result {
      if true then tIf.Trigger() else tElse.Trigger()
      t.Trigger()
    }

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

    let result = result {
      if false then tIf.Trigger() else tElse.Trigger()
      t.Trigger()
    }

    test <@ result = Ok () @>
    test <@ not tIf.Triggered @>
    test <@ tElse.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``behavior of if-else with wrapped ok value`` () =
  Property.check <| property {
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result = result {
      if true then
        do! Ok ()
        tIf.Trigger()
      else tElse.Trigger()
      t.Trigger()
    }

    test <@ result = Ok () @>
    test <@ tIf.Triggered @>
    test <@ not tElse.Triggered @>
    test <@ t.Triggered @>
  }

[<Fact>]
let ``behavior of if-else with wrapped error value`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tIf = Trigger()
    let tElse = Trigger()
    let t = Trigger()

    let result = result {
      if true then
        do! Error err
        tIf.Trigger()
      else tElse.Trigger()
      t.Trigger()
    }

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

    let result = result {
      for t in ts do
        t.Trigger()
    }

    test <@ result = Ok () @>
    test <@ ts |> List.forall (fun t -> t.Triggered) @>
  }


[<Fact>]
let ``behavior of for when some items fail`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let ts =
      [(Trigger(), Ok (), Trigger())
       (Trigger(), Error err, Trigger())
       (Trigger(), Ok (), Trigger())]

    let result = result {
      for t1, res, t2 in ts do
        t1.Trigger()
        do! res
        t2.Trigger()
    }

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

    let result = result {
      while enum.MoveNext () do
        enum.Current.Trigger()
    }

    test <@ result = Ok () @>
    test <@ ts |> List.forall (fun t -> t.Triggered) @>
  }


[<Fact>]
let ``behavior of while when some items fail`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let ts =
      [(Trigger(), Ok (), Trigger())
       (Trigger(), Error err, Trigger())
       (Trigger(), Ok (), Trigger())]
    use enum = (ts |> Seq.ofList).GetEnumerator()

    let result = result {
      while enum.MoveNext () do
        let t1, res, t2 = enum.Current
        t1.Trigger()
        do! res
        t2.Trigger()
    }

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

    let result = result {
      try
        try tTry.Trigger()
        with _ -> tCatch.Trigger()
      finally tFinally.Trigger()
    }

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

    let result = result {
      try
        try
          failwith ""
          tTry.Trigger()
        with _ -> tCatch.Trigger()
      finally tFinally.Trigger()
    }

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

    let result = result {
      use _d = disp
      ()
    }

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``simple use disposes when result is ok`` () =
  Property.check <| property {
    let tDisp = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger()}

    let result = result {
      use _d = disp
      do! Ok ()
    }

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``simple use disposes when result is error`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let tDisp = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger() }

    let result = result {
      use _d = disp
      do! Error err
    }

    test <@ result = Error err @>
    test <@ tDisp.Triggered @>
  }


[<Fact>]
let ``use! continues and disposes when disposable is ok`` () =
  Property.check <| property {
    let tDisp = Trigger()
    let t = Trigger()
    let disp = { new IDisposable with member __.Dispose () = tDisp.Trigger()}

    let result = result {
      use! _d = Ok disp
      t.Trigger()
    }

    test <@ result = Ok () @>
    test <@ tDisp.Triggered @>
    test <@ t.Triggered @>
  }


[<Fact>]
let ``use! stops when disposable is error`` () =
  Property.check <| property {
    let! err = GenX.auto<string>
    let t = Trigger()

    let result = result {
      use! _d = Error err
      t.Trigger()
    }

    test <@ result = Error err @>
    test <@ not t.Triggered @>
  }


[<Fact>]
let ``use ignores null disposable`` () =
  Property.check <| property {
    let result = result {
      use _d = null
      do! Ok ()
    }
    test <@ result = Ok () @>
  }


[<Fact>]
let ``use! ignores null disposable`` () =
  Property.check <| property {
    let result = result {
      use! _d = Ok null
      do! Ok ()
    }
    test <@ result = Ok () @>
  }


[<Fact>]
let ``use! handles non-nullable disposable`` () =
  Property.check <| property {
    let result = result {
      use! _d = Ok (new CustomDisposable())
      do! Ok ()
    }    
    raises <@ CustomDisposedException @>
  }