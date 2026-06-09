# Skill Improvements Summary

**Date**: 2026-05-20
**Retro files processed**:
- `2026-05-20-missed-skill.md`

---

## Changes Made

### Trigger Phrase Expansion (YAML frontmatter `description:`)

Added the following trigger phrases to the `description:` field to improve skill activation coverage:
`"generate XtraReport"`, `"group report by field"`, `"grouped by category"`, `"bind report to collection"`,
`"programmatically build report"`, `"dynamic report creation"`, `"runtime report generation"`,
`"DevExpress report from code"`, `"add control to report"`.  
Addresses: `2026-05-20-missed-skill.md`

### New CRITICAL Callout (When to Use This Skill section)

Added a `> CRITICAL — Do not bypass this skill.` blockquote at the top of "When to Use This Skill" instructing agents to follow skill patterns and reference files instead of their own general DevExpress API knowledge.  
Addresses: `2026-05-20-missed-skill.md`

### New Constraint — Rule 13 (Constraints & Rules section)

**Rule 13 — Follow skill patterns, not general knowledge**: When this skill is active, implement using the patterns and examples in the skill and its reference files. Never silently fall back to general AI knowledge. If a needed API is not found, state that explicitly rather than guessing.  
Addresses: `2026-05-20-missed-skill.md`

---

## Root Causes Fixed

1. **Skill not triggered by task description**: The `description:` lacked phrases like "group report by field", "programmatically build report", "DevExpress report from code" that commonly appear in user requests. Expanding the trigger phrase list increases the probability the skill activates for all XtraReports coding tasks.

2. **Agent bypassed skill using own knowledge**: Even when a skill is loaded, an agent can ignore it and rely on general knowledge. Added a visible CRITICAL note at the skill's entry point and Constraint 13 to establish the rule explicitly inside the skill body.

---

## Validation Checklist

- [x] No new contradictions in existing CRITICAL constraints (Rules 1–12 unchanged)
- [x] Skill boundary (runtime API only, no viewer UI) is preserved
- [x] Retro finding is addressed
- [x] Version number not auto-bumped (metadata.version tracks DevExpress product versioning per repo convention)
- [x] A hypothetical agent reading the updated skill would: be triggered by grouping/binding requests; see the CRITICAL note and follow skill patterns rather than their own knowledge
