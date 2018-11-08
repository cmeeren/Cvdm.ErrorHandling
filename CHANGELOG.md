Changelog
===

##### 2.0.0 (2018-11-08)

- Breaking: `asyncResult` computation is now lazy; just defining it won't do anything.
- Possibly breaking: Computation expressions have been rewritten. Apart from the previously mentioned laziness, all old and new unit tests (fairly extensive) pass for both 1.0.2 and 2.0.0. However, I can not guarantee there aren't breaks in untested edge cases.
- `asyncResult` can now bind  `Task<_>` and `Task` expressions

##### 1.0.2 (2018-09-15)

- Fix weird design-time crash when used if project code is run in XAML designer

##### 1.0.1 (2018-08-05)

* Fix `try ... with`/`try ... finally` in `asyncResult` not catching exceptions thrown from plain `async` expressions

##### 1.0.0 (2018-07-05)

* Support Fable
* Add more helper functions

##### 0.5.1 (2018-03-14)

* Fix `use`/`use!` in conjunction with non-nullable `IDisposable` types

##### 0.5.0 (2018-03-14)

* Breaking: Added assembly-level AutoOpen attribute. Importing the library will automatically import the helper methods as well as the builder instances

##### 0.4.0 (2018-03-08)

* Breaking: Rename `orElse` and `orElseWith` to `defaultValue` and `defaultWith` so the names match the functions with similar signatures in the `Option` module
* Add `Result.ignoreError` and `AsyncResult.ignoreError`

##### 0.3.0 (2018-03-08)

* Add `Result.requireNone`, `AsyncResult.requireTrue`, `AsyncResult.requireFalse`, `AsyncResult.requireSome`, `AsyncResult.requireNone`
* Fix package tags

##### 0.2.0 (2018-03-07)

* Add `setError`
* Fix package tags

##### 0.1.1 (2018-03-07)

* Add package metadata

##### 0.1.0 (2018-03-07)

* Initial release
