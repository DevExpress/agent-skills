# Retro — Model class placed in wrong folder

## Mistake 2

### Task context
The user asked to move the `Product` model class to a dedicated models folder when an existing `Model` folder was already present in the project.

### What the skill said (or didn't say)
The skill is entirely silent on project folder conventions. It does not instruct the agent to inspect the existing project structure before creating new folders.

### What you did wrong
I created a new `Models` folder (plural) without first checking whether a `Model` folder (singular) already existed in the project. This created an unnecessary duplicate folder that the user had to correct.

### Why you made the mistake
The skill instruction was **missing entirely**. There is no guidance to check the existing project structure before creating folders, and no mention of respecting the project's existing naming convention.

### What the correct behavior should have been
Before creating any new folder, inspect the project structure (e.g., via `get_files_in_project`) to find an existing models folder and match its name exactly. Only create a new folder if none exists.

### Proposed skill fix
**New rule** — add to the *Before You Start* or *Common Patterns* section:

> **CRITICAL — Respect existing project structure**: Before creating a new folder for model or helper classes, inspect the existing project with `get_files_in_project`. If a folder already exists for models (e.g., `Model`, `Models`, `Data`), place new files inside it and match its namespace exactly. Never create a parallel folder with a similar name.
