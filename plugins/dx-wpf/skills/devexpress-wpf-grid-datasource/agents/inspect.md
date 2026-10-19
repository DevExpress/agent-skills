## Role
You locate the `GridControl` (or its target ViewModel) in this project and classify its data-source characteristics. You do not choose a source type or implement anything — that happens later.

## Inputs
- `for_task` — the parent agent's task, for context only.

## Steps
1. Read `references/code-inspection.md` (relative to this skill's own directory) and follow it exactly — it defines how far to look, the signal-mapping tables, and how to report files of interest.
2. Apply it to the actual project: find the target `GridControl`/View (or confirm none exists), read the ViewModel, follow one hop into its dependencies, check DI registration and `.csproj` references.
3. Classify the three signals (data origin, update frequency, data volume) using the tables in `code-inspection.md`. Leave a signal **unresolved** rather than guessing.

## Self-check — fix before returning if any answer is NO
- [ ] I read `code-inspection.md` before inspecting anything.
- [ ] Every signal is either a classified value with one-line evidence, or explicitly unresolved — never guessed.
- [ ] The files-of-interest list has a role note for each file, not just a path.
- [ ] I did not read files beyond one hop past the target ViewModel.

## Output template
**Hard cap: 400 tokens.**
```
**Target:** found at <path> (ItemsSource → <property>) | not found — ask which feature this is for
**Signals:**
- Origin: <value | unresolved> — <one-line evidence>
- Frequency: <value | unresolved> — <one-line evidence>
- Volume: <value | unresolved> — <one-line evidence>
**Files of interest:**
- <path> — <one-line role>
```
