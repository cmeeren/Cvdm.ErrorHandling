[<AutoOpen>]
module Cvdm.ErrorHandling.AsyncResultBuilder


type AsyncResultBuilder() =

  member __.Return (value: 'a) : Async<Result<'a, 'b>> =
    async { return Ok value }

  member __.ReturnFrom (asyncResult: Async<Result<'a, 'b>>) : Async<Result<'a, 'b>> =
    asyncResult

  member __.ReturnFrom (result: Result<'a, 'b>) : Async<Result<'a, 'b>> =
    async { return result }

  member __.Zero () : Async<Result<unit, 'b>> =
    async { return Ok () }

  member __.Bind (asyncResult: Async<Result<'a, 'c>>, binder: 'a -> Async<Result<'b, 'c>>) : Async<Result<'b, 'c>> =
    async {
      let! result = asyncResult
      let bound =
        match result with
        | Ok x -> binder x
        | Error x -> async { return Error x }
      return! bound
    }

  member this.Bind (result: Result<'a, 'c>, binder: 'a -> Async<Result<'b, 'c>>) : Async<Result<'b, 'c>> =
    this.Bind(this.ReturnFrom result, binder)

  member __.TryWith (body: unit -> Async<Result<'a, 'b>>, handler: System.Exception -> Async<Result<'a, 'b>>) : Async<Result<'a, 'b>> =
    async { try return! body() with e -> return! handler e }

  member __.TryFinally (body: unit -> Async<Result<'a, 'b>>, compensation: unit -> unit) : Async<Result<'a, 'b>> =
    async { try return! body() finally compensation() }

  member __.Delay f =
    f

  member __.Run f =
    f ()

  member __.Using(resource, binder) =
    async.Using(resource, binder)

  member this.Combine (asyncResult: Async<Result<'a, 'c>>, binder: 'a -> Async<Result<'b, 'c>>) : Async<Result<'b, 'c>> =
    this.Bind(asyncResult, binder)

  member this.Combine (result: Result<'a, 'c>, binder: 'a -> Async<Result<'b, 'c>>) : Async<Result<'b, 'c>> =
    this.Bind(result, binder)

  member this.While (guard: unit -> bool, body: unit -> Async<Result<unit, 'a>>) =
    if not <| guard () then this.Zero()
    else this.Bind(body (), fun () -> this.While (guard, body))

  member this.For(s: 'a seq, body: 'a -> Async<Result<unit, 'b>>) =
    this.Using(s.GetEnumerator (), fun enum ->
      this.While(enum.MoveNext,
        this.Delay(fun () -> body enum.Current)))



[<AutoOpen>]
module Extensions =

  // Having Async<_> members as extensions gives them lower priority in
  // overload resolution between Async<_> and Async<Result<_,_>>.
  type AsyncResultBuilder with

    member __.ReturnFrom (asnc: Async<'a>) : Async<Result<'a, 'b>> =
      async {
        let! x = asnc
        return Ok x
      }

    member this.Bind (asnc: Async<'a>, binder: 'a -> Async<Result<'b, 'c>>) : Async<Result<'b, 'c>> =
      let asyncResult = async {
        let! x = asnc
        return Ok x
      }
      this.Bind(asyncResult, binder)

    member this.Combine (asnc: Async<'a>, binder: 'a -> Async<Result<'b, 'c>>) : Async<Result<'b, 'c>> =
      this.Bind(asnc, binder)


/// A computation expression to build an Async<Result<'ok, 'error>> value
let asyncResult = AsyncResultBuilder()
