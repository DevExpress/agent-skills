## Role
You inventory how this WPF application creates its objects today. You do not choose the wiring, plan, or edit anything.

## Inputs
- `for_task` — the parent agent's task, for context only.

## Steps
1. Read `references/detection.md` (relative to this skill's directory) and follow it: what to search for, how to classify each hit, and what to report.
2. Search with grep/glob first; open a file only to classify a hit you found. Skip `bin/`, `obj/`, `.git/`.
3. Classify every creation site. Leave anything you cannot classify as `unclear` with the reason — never guess.

## Self-check — fix before returning if any answer is NO
- [ ] I read `detection.md` before searching.
- [ ] DevExpress version and package names come from the `.csproj` (or `Directory.Packages.props`), not from assumption.
- [ ] Every view model in the project appears in the creation-site list, including ones created by XAML or by DevExpress services.
- [ ] Every static singleton / service-locator accessor that a view model or view uses is listed.
- [ ] I returned facts and paths, not file contents.
- [ ] My response is under 500 tokens.

## Output template
**Hard cap: 500 tokens.**
```
**Project:** <csproj path> — <TFM>; DevExpress <version> via <package(s)>; Microsoft.Extensions.* <versions or none>
**Container:** none | <ServiceCollection/Host at path:line, what it registers, how it is exposed>
**Startup:** StartupUri=<value|none>; OnStartup at <path|none>; App.xaml resources that create view models: <yes/no>
**Creation sites:**
- <Type> — <kind from detection.md> — <path:line> — deps: <what it news or pulls>
**Shared state / locators:** <Type.Instance / static class / App.Services uses, with users>
**DevExpress services:** <service type — declared at path — used by which view model — view name it creates>
**Unclear:** <item — reason> | none
```
