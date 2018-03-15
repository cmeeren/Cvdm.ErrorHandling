[<AutoOpen>]
module Cvdm.ErrorHandling.ResultBuilder

open System

type ResultBuilder() =

  member __.Return value =
    Ok value

  member __.ReturnFrom value =
    value

  member __.Bind (result, binder) =
    Result.bind binder result

  member this.Zero () =
    this.Return ()

  member __.Delay f =
    f

  member __.Run f =
    f ()

  member this.Combine (result, binder) =
    this.Bind(result, binder)

  member __.TryWith (body, handler) =
    try body () with | e -> handler e

  member __.TryFinally (body, compensation) =
    try body () finally compensation ()

  member x.Using (disp: #IDisposable, body) =
    let result = fun () -> body disp
    x.TryFinally (result, fun () ->
      match disp with
      | null -> ()
      | d -> d.Dispose ())

  member this.While (guard, body) =
    if not <| guard () then this.Zero()
    else this.Bind(body (), fun () -> this.While (guard, body))

  member this.For(s: seq<_>, body) =
    this.Using(s.GetEnumerator (), fun enum ->
      this.While(enum.MoveNext,
        this.Delay(fun () -> body enum.Current)))

