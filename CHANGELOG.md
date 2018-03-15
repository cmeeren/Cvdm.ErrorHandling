Changelog
===

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

