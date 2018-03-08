Cvdm.ErrorHandling
===

[![NuGet](https://img.shields.io/nuget/dt/Cvdm.ErrorHandling.svg?style=flat)](https://www.nuget.org/packages/Cvdm.ErrorHandling/) [![Build status](https://ci.appveyor.com/api/projects/status/r4pe0qp93fnjenoc/branch/master?svg=true)](https://ci.appveyor.com/project/cmeeren/cvdm-errorhandling/branch/master)

*`asyncResult` and `result` computation expressions and helper functions for error handling in F#.*



The `result` computation expression
---

The `result` computation expression simplifies synchronous error handling using F#'s `Result<'a,'b>` type. A single computation expression must have a single error type, and helper functions simplify transforming return values to a `Result` with the needed error type. Here's an example:

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

The `asyncResult` computation expression is more or less identical to the `result` expression except it's centered around `Async<Result<'a,'b>> `, with overloads supporting `Result<'a,'b>` (which is wrapped in `Async`) and `Async<'a>` (which is wrapped using `Result.Ok`). In other words, on the right side of `let!`, `do!` etc. you can have `Async<'a>`, `Async<Result<'a, 'b>` or `Async<'a>`.

`asyncResult` is intended to be almost a drop-in replacement of `result`. If you have a `result` expression and you need to unwrap `Async` values, just change to `asyncResult`. (The consumers of the changed expression will of course now need to change since it's `Async<Result<'a,'b>>` instead of `Result<'a,'b>`, but the contents of the expression itself should not need to change just from this switch.)

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

#### A note on overload resolution

(It all "just works" as you'd want; this is for the curious.)

If you have an expression of type `Async<Result<'a,'b>>`, then the compiler normally doesn't know how to choose between the overloads taking `Async<Result<'a,'b>>` and `Async<'a>` since both are compatible. I have solved this by having the `Async<'a>` members as extension methods. The compiler will then give these lower priority. I consider this bit of "magic" completely acceptable in this situation since

1. the resulting behavior is intuitive and exactly what you want,
2. it still follows strict rules (as defined above), and
3. the whole point of this computation expression is to make your life easier when handling asynchronous errors. The alternative is to explicitly wrap all `Async<Result<'a,'b>>` expressions in a wrapper type to make overload resolution work (like Chessie does) which IMHO doesn't really add any meanungful clarity.

The helper functions
---

See [Helpers.fs](https://github.com/cmeeren/Cvdm.ErrorHandling/blob/master/Cvdm.ErrorHandling/Helpers.fs).