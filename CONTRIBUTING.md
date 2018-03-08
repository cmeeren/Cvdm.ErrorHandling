Deployment checklist
===

1. Make necessary changes to the code
2. Ensure new functionality has tests and that the tests pass
3. Update the changelog
4. Update the version and release notes in the package info
5. Commit and tag the commit (this is what triggers deployment from AppVeyor). For consistency, the tag should ideally be in the format `v1.2.3`.
6. Push the changes and the tag to the repo. If AppVeyor build/testing succeeds, the package is automatically published to NuGet.