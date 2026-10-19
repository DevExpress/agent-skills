# Steps 8–9: Apply and Verify

## Step 8: Apply the Customization

### Target file and location

| Scenario | Location |
|----------|----------|
| App-wide, or element appears in popups / MessageBoxes | `App.xaml` → `<Application.Resources>` |
| Window- or control-scoped | The root element's `.Resources` in that file |

**Important:** Resources must be declared *before* the elements that use them.

---

### Option A — Inline into existing Resources

**In agent mode:**
1. `grep` the target file for `Application.Resources` to get the line number — do not load the full file
2. Read ~10 lines around that line with `view_range` to inspect the structure:
   - **Bare resources** (`<Application.Resources>` directly contains resource elements) → insert the new resource(s) inside, before the closing tag
   - **Wrapped** (`<Application.Resources>` contains a `<ResourceDictionary>` child) → insert inside that `<ResourceDictionary>`, not alongside it. Mixing bare resources with a `<ResourceDictionary>` wrapper causes a WPF runtime error.
3. Insert the customized resource(s) and static dependencies at the correct position using `edit`
4. Add any missing XML namespace declarations to the root element

**In read-only / manual mode:** Provide a complete XAML block with namespace declarations, the resource(s), and clear paste instructions.

---

### Option B — Separate ResourceDictionary

**In agent mode:**
1. Create the new file (e.g., `Themes/DxOverrides.xaml`) with a `ResourceDictionary` root containing all required namespaces and the customized resource(s)
2. In `App.xaml`, merge the new dictionary:
   ```xml
   <Application.Resources>
       <ResourceDictionary>
           <ResourceDictionary.MergedDictionaries>
               <ResourceDictionary Source="Themes/DxOverrides.xaml"/>
           </ResourceDictionary.MergedDictionaries>
       </ResourceDictionary>
   </Application.Resources>
   ```
   If `<ResourceDictionary.MergedDictionaries>` already exists, append — do not replace.
3. **Ensure the new file's Build Action is `Page`** (not `Resource` or `None`):
   - **SDK-style `.csproj`** — `.xaml` files are auto-included as `Page` by default; no explicit entry needed. To check: `view_range` the first line of the `.csproj` — if it contains `Sdk=`, it's SDK-style.
   - **Non-SDK `.csproj`** (old-style, no `Sdk=` on the first line) — an explicit entry is required. Check with `grep` for the file in the `.csproj`, and add if missing:
     ```xml
     <Page Include="Themes\DxOverrides.xaml" />
     ```

**In read-only / manual mode:** Provide both files' content with clear step-by-step paste instructions and a note about the `Page` Build Action.

---

## Step 9: Verify

After applying changes, do a final sanity check:

1. **Namespaces** — every XML namespace prefix used inside the customized resource must be declared on the root element of the file it lives in. Grep the edited file for `xmlns:` declarations and cross-check that each prefix used inside the resource is covered; flag any that are missing.
2. **Duplicate keys** — grep the target file (and any merged dictionaries) for the same `x:Key` value. A duplicate key causes a silent override or a runtime parse error.
3. **Build Action** — if Option B was used, confirm the `Page` Build Action per the rules above.
4. **Build it yourself** — in agent mode run the build (`dotnet build`); don't hand that to the user. XAML parse errors (missing namespace prefix, typo in a resource key) surface only at build or runtime and no static check catches them. A green build still does not prove the change renders — Step 10 does that. In read-only / manual mode, ask the user to run *Build Solution* and report back.
