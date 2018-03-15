[<AutoOpen>]
module Cvdm.ErrorHandling.AsyncResultBuilder


type AsyncResult<'a,'b> = AR of Async<Result<'a,'b>> with
  member this.ToAsync = let (AR ar) = this in ar


module AsyncResult =

  let toAsync (AR ar) =
    ar


type AsyncResultBuilder() =

  member __.Return (value: 'a) : AsyncResult<'a, 'b> =
    async { return Ok value } |> AR

  member __.ReturnFrom (asyncResult: AsyncResult<'a, 'b>) : AsyncResult<'a, 'b> =
    asyncResult

  member __.ReturnFrom (asnc: Async<'a>) : AsyncResult<'a, 'b> =
    async {
      let! x = asnc
      return Ok x
    } |> AR

  member __.ReturnFrom (result: Result<'a, 'b>) : AsyncResult<'a, 'b> =
    async { return result } |> AR

  member __.Zero () : AsyncResult<unit, 'b> =
    async { return Ok () } |> AR

  member __.Bind (asyncResult: AsyncResult<'a, 'c>, binder: 'a -> AsyncResult<'b, 'c>) : AsyncResult<'b, 'c> =
    async {
      let (AR ar) = asyncResult
      let! result = ar
      let bound =
        match result with
        | Ok x -> binder x |> AsyncResult.toAsync
        | Error x -> async { return Error x }
      return! bound
    } |> AR

  member this.Bind (asnc: Async<'a>, binder: 'a -> AsyncResult<'b, 'c>) : AsyncResult<'b, 'c> =
    let asyncResult =
      async {
        let! x = asnc
        return Ok x
      } |> AR
    this.Bind(asyncResult, binder)

  member this.Bind (result: Result<'a, 'c>, binder: 'a -> AsyncResult<'b, 'c>) : AsyncResult<'b, 'c> =
    this.Bind(this.ReturnFrom result, binder)

  member __.TryWith (body, handler) =
    try body() with e -> handler e

  member __.TryFinally (body, compensation) =
    try body() finally compensation()

  member __.Delay f =
    f

  member __.Run f =
    f ()

  member __.Using(resource, binder) =
    async.Using(resource, binder >> AsyncResult.toAsync) |> AR

  member this.Combine (asyncResult: AsyncResult<'a, 'c>, binder: 'a -> AsyncResult<'b, 'c>) : AsyncResult<'b, 'c> =
    this.Bind(asyncResult, binder)

  member this.Combine (asnc: Async<'a>, binder: 'a -> AsyncResult<'b, 'c>) : AsyncResult<'b, 'c> =
    this.Bind(asnc, binder)

  member this.Combine (result: Result<'a, 'c>, binder: 'a -> AsyncResult<'b, 'c>) : AsyncResult<'b, 'c> =
    this.Bind(result, binder)

  member this.While (guard: unit -> bool, body: unit -> AsyncResult<unit, 'a>) : AsyncResult<unit, 'a> =
    if not <| guard () then this.Zero()
    else this.Bind(body (), fun () -> this.While (guard, body))

  member this.For(s: 'a seq, body: 'a -> AsyncResult<unit, 'b>) : AsyncResult<unit, 'b> =
    this.Using(s.GetEnumerator (), fun enum ->
      this.While(enum.MoveNext,
        this.Delay(fun () -> body enum.Current)))
