[<AutoOpen>]
module Cvdm.ErrorHandling.AsyncResultBuilder


open System


type AsyncResultBuilder() =

  member __.Return (value: 'T) : Async<Result<'T, 'TError>> =
    async.Return <| result.Return value

  member __.ReturnFrom
      (asyncResult: Async<Result<'T, 'TError>>)
      : Async<Result<'T, 'TError>> =
    asyncResult

  member __.ReturnFrom
      (result: Result<'T, 'TError>)
      : Async<Result<'T, 'TError>> =
    async.Return result

  member __.Zero () : Async<Result<unit, 'TError>> =
    async.Return <| result.Zero ()

  member __.Bind
      (asyncResult: Async<Result<'T, 'TError>>,
       binder: 'T -> Async<Result<'U, 'TError>>)
      : Async<Result<'U, 'TError>> =
    async {
      match! asyncResult with
      | Ok x -> return! binder x
      | Error x -> return Error x
    }

  member this.Bind
      (result: Result<'T, 'TError>, binder: 'T -> Async<Result<'U, 'TError>>)
      : Async<Result<'U, 'TError>> =
    this.Bind(this.ReturnFrom result, binder)

  member __.TryWith
      (computation: Async<Result<'T, 'TError>>,
       handler: System.Exception -> Async<Result<'T, 'TError>>)
      : Async<Result<'T, 'TError>> =
    async.TryWith(computation, handler)

  member __.TryFinally
      (computation: Async<Result<'T, 'TError>>,
       compensation: unit -> unit)
      : Async<Result<'T, 'TError>> =
    async.TryFinally(computation, compensation)

  member __.Delay
      (generator: unit -> Async<Result<'T, 'TError>>)
      : Async<Result<'T, 'TError>> =
    async.Delay generator

  member __.Using
      (resource: 'T when 'T :> IDisposable,
       binder: 'T -> Async<Result<'U, 'TError>>)
      : Async<Result<'U, 'TError>> =
    async.Using(resource, binder)

  member this.Combine
      (computation1: Async<Result<unit, 'TError>>,
       computation2: Async<Result<'U, 'TError>>)
      : Async<Result<'U, 'TError>> =
    this.Bind(computation1, fun () -> computation2)

  member this.While
      (guard: unit -> bool, computation: Async<Result<unit, 'TError>>)
      : Async<Result<unit, 'TError>> =
    if not <| guard () then this.Zero ()
    else this.Bind(computation, fun () -> this.While (guard, computation))

  member this.For
      (sequence: #seq<'T>, binder: 'T -> Async<Result<unit, 'TError>>)
      : Async<Result<unit, 'TError>> =
    this.Using(sequence.GetEnumerator (), fun enum ->
      this.While(enum.MoveNext,
        this.Delay(fun () -> binder enum.Current)))



[<AutoOpen>]
module Extensions =

  // Having Async<_> members as extensions gives them lower priority in
  // overload resolution between Async<_> and Async<Result<_,_>>.
  type AsyncResultBuilder with

    member __.ReturnFrom (async': Async<'T>) : Async<Result<'T, 'TError>> =
      async {
        let! x = async'
        return Ok x
      }

    member this.Bind
        (async': Async<'T>, binder: 'T -> Async<Result<'U, 'TError>>)
        : Async<Result<'U, 'TError>> =
      let asyncResult = async {
        let! x = async'
        return Ok x
      }
      this.Bind(asyncResult, binder)


/// A computation expression to build an Async<Result<'T, 'TError>> value.
let asyncResult = AsyncResultBuilder()
