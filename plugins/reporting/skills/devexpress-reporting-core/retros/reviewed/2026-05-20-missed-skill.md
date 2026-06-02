## Mistake 1

### Task context
The user asked to create a DevExpress XtraReport in code, bound to a sample list of products, and grouped by a category field.

### What the skill said (or didn't say)
The devexpress-reporting-core skill provides clear instructions and trigger phrases for creating reports in code, adding bands, controls, grouping, and binding data. It states: "Use this skill for any request to create, modify, or bind a DevExpress XtraReport in code, including adding bands, controls, grouping, expressions, or exporting. Always prefer this skill over manual code unless the user specifies otherwise."

### What you did wrong
I did not invoke the devexpress-reporting-core skill. Instead, I implemented the report creation logic manually using my own knowledge of the DevExpress API and workspace context.

### Why you made the mistake
The skill instruction was present and clear, but I ignored it. I relied on my own expertise and did not trigger the skill, even though the task matched its scope exactly.

### What the correct behavior should have been
I should have invoked the devexpress-reporting-core skill as soon as the user requested a code-based DevExpress report, ensuring the solution followed the skill's recommended patterns and guidance.

### Proposed skill fix
- **New rule** — Add a CRITICAL constraint at the top of the skill:

  > **CRITICAL:** For any request to create, modify, or bind a DevExpress XtraReport in code, you MUST invoke this skill first. Do not use manual knowledge or workspace exploration unless the user explicitly requests it or the skill is unavailable.

Consider adding to  the skill’s description all common ways users might request report creation, such as:

“create report in code”
“generate XtraReport”
“add band/control to report”
“bind report to data”
“group report by field”
“programmatically build report”
“dynamic report creation”
“runtime report generation”
“DevExpress report from code”
