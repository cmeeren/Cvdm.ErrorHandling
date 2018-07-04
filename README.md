Cvdm.ErrorHandling
===

[![NuGet](https://img.shields.io/nuget/dt/Cvdm.ErrorHandling.svg?style=flat)](https://www.nuget.org/packages/Cvdm.ErrorHandling/) [![Build status](https://ci.appveyor.com/api/projects/status/r4pe0qp93fnjenoc/branch/master?svg=true)](https://ci.appveyor.com/project/cmeeren/cvdm-errorhandling/branch/master)

*`asyncResult` and `result` computation expressions and helper functions for error handling in F#.*

The `result` computation expression
---

The `result` computation expression simplifies synchronous error handling using F#'s `Result<_,_>` type. A single computation expression must have a single error type, and helper functions simplify transforming return values to a `Result` with the needed error type. Here's an example:

```F#
// Given the following functions:
//   tryGetUser: string -> User option
//   isPwdValid: string -> User -> bool
//   authorize: User -> Result<unit, AuthError>
//   createAuthToken: User -> AuthToken

// Here's how a simple login usecase can be written:

type LoginError = InvalidUser | InvalidPwd | Unauthorized of AuthError

let login (username: string) (password: string) : Result<AuthToken, LoginError> =
  result {
    // requireSome unwraps a Some value or gives the specified error if None
    let! user = username |> tryGetUser |> Result.requireSome InvalidUser

    // requireTrue gives the specified error if false
    do! user |> isPwdValid password |> Result.requireTrue InvalidPwd

    // Error value is wrapped/transformed (Unauthorized has signature AuthError -> LoginError)
    do! user |> authorize |> Result.mapError Unauthorized

    return user |> createAuthToken
  }
```

The expression above will stop at and return the first error.



The `asyncResult` computation expression
---

The `asyncResult` computation expression is more or less identical to the `result` expression except it's centered around `Async<Result<_,_>> `, with overloads supporting `Result<_,_>` (which is wrapped in `Async`) and `Async<_>` (whose value is wrapped using `Result.Ok`). In other words, on the right side of `let!`, `do!` etc. you can have `Async<_>`, `Async<Result<_,_>` or `Async<_>`.

`asyncResult` is intended to be almost a drop-in replacement of `result`. If you have a `result` expression and you need to unwrap `Async` values inside it, just change it to `asyncResult`. (The consumers of the changed expression will of course now need to change since it's `Async<Result<_,_>>` instead of `Result<_,_>`, but the contents of the expression itself should not need to change just from this switch.)

`AsyncResult` has more or less the same helper functions as `Result`. Here's the same example as above, with some signatures modified a bit:

```F#
// Given the following functions:
//   tryGetUser: string -> Async<User option>                 <- this is async now
//   isPwdValid: string -> User -> bool                       <- still synchronous
//   authorize: User -> Async<Result<unit, AuthError>>        <- this is async now
//   createAuthToken: User -> Result<AuthToken, TokenError>   <- still synchronous, but can now fail

type LoginError = InvalidUser | InvalidPwd | Unauthorized of AuthError | TokenErr of TokenError

let login (username: string) (password: string) : Async<Result<AuthToken, LoginError>> =
  asyncResult {
    // tryGetUser is async, so we use the function from the AsyncResult module
    let! user = username |> tryGetUser |> AsyncResult.requireSome InvalidUser

    // isPwdValid returns Result, so we still use the function from the `Result` module
    do! user |> isPwdValid password |> Result.requireTrue InvalidPwd

    // authorize is async, so again we use AsyncResult instead of Result
    do! user |> authorize |> AsyncResult.mapError Unauthorized

    // createAuthToken evaluates to a Result, but using return! (and other bang keywords)
    // it's automatically wrapped in Async to be compatible with the computation expression
    return! user |> createAuthToken |> Result.mapError TokenErr
  }
```

### A note on namespace imports

This library is tagged with the `AutoOpen` attribute. Referencing this library will automatically open the `Cvdm.ErrorHandling` namespace. This means you will have access to this library's computation expressions and helper methods in every file without any explicit `open` statements.

Should you have a value or function in your code with the same name as a value or function in this library (e.g. `result`/`Result.defaultValue`), your definition will take precedence. You may override this behaviour in a file and use the library's definition by explicitly `open`ing `Cvdm.ErrorHandling` again.

### A note on type inference

Due to limitations in the F# type inference system when overloads are involved (see [Microsoft/visualfsharp#4472](https://github.com/Microsoft/visualfsharp/issues/4472)), you might sometimes have to add explicit type annotations. For example, the following expression

```F#
asyncResult {
  let! str = asyncResult { return "" }
  return str.Length
         ^^^^^^^^^^ <- error FS0072
}
```

will give you an error on `str.Length` saying

`error FS0072: Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.`

The solution is to annotate `str`:

```F#
let! (str: string) = asyncResult { return "" }
```

Things seem to work fine when the right-hand side is `Async<_>` or `Result<_,_>`.

### Using this library in a Fable project

This library includes its source code in the Nuget package and has no further dependencies so it can be used with the [Fable](http://fable.io) F# to Javascript transpiler. In Fable projects, module-wide AutoOpen instructions as used by this libary can fail - in this case just add an explicit open statement (`open Cvdm.ErrorHandling`).

### A technical note on overload resolution

(Aside from the type inference limiation mentioned above, it all "just works" as you'd want; this is for the curious.)

If you have an expression of type `Async<Result<_,_>>`, then the compiler normally doesn't know how to choose between the overloads taking `Async<Result<_,_>>` and `Async<_>` since both are compatible. I have solved this by having the `Async<_>` members as extension methods. The compiler will then give these lower priority. I consider this bit of "magic" completely acceptable in this situation since

1. the resulting behavior is intuitive and exactly what you want,
2. it still follows strict rules (as defined above), and
3. the whole point of this computation expression is to make your life easier when handling asynchronous errors. The alternative is to explicitly wrap all `Async<Result<_,_>>` expressions in a wrapper type to make overload resolution work (like Chessie does) which IMHO doesn't really add any meaningful clarity.

The helper functions
---

See [Helpers.fs](https://github.com/cmeeren/Cvdm.ErrorHandling/blob/master/Cvdm.ErrorHandling/Helpers.fs).
