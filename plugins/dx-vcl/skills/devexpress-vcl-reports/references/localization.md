# Localization: UI language, document culture, report content

See the public example `vcl-reports-localize`.

## UI language (Report Viewer / Designer)

- Set `TdxReport.Language` / `TdxReportControl.Language` to a culture name, e.g. `'de-DE'`. It is passed to the web UI
  as the browser language and a `Language` cookie, and re-read on every web resource request.
- **Translations are JSON files deployed next to the exe:**
  - `<exe folder>\localization\<name>.<Language>.json`; if that file is missing, the two-letter fallback
    `<name>.de.json` is tried;
  - names requested by the Viewer: `dx-analytics-core`, `dx-reporting`;
  - the Designer also requests `dx-rich`;
  - Use the **hyphenated** names (`dx-reporting.de.json`).
- **A missing file fails silently.** The backend returns `{}` and the UI stays English. Check the `localization\`
  folder first.
- **The path is the exe folder** (`TPath.GetAppPath`), not the current directory. Every build configuration that writes
  the exe elsewhere needs its own copy of `localization\`.

## Document culture (exports and preview)

- `Language` also sets the **document culture** used by `ExportTo*`: dates, numbers and currency.
  - `de-DE`: `Sonntag, 15. März 2026`, `1.234,50`, `1.234,50 €`;
  - `fr-FR`: `1 234,50 €`.
- A neutral culture (`de`) formats currency with the generic sign `¤`. Use specific cultures (`de-DE`).
- Changing `Language` between exports of the same report takes effect immediately.

## Report content (translated texts inside the layout)

- Content translations are stored in the REPX as `<LocalizationItems>`. Each item has a `Component="#Ref-N"`,
  `Culture="de"` / `"Default"`, `Path="Text"` and `Data="..."` value. The item matching `Language` (with the two-letter
  fallback) is chosen.
- **Don't hand-write partial `LocalizationItems`.** A block with only `Text` items resets every other localizable
  property (sizes, locations, formats) to its default. The Designer's Localization editor writes
  full `Culture="Default"` sets for every localizable property, so author translations there.
