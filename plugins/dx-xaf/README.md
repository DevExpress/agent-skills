# XAF Skills

AI agent skills for [DevExpress XAF](https://docs.devexpress.com/eXpressAppFramework/) (eXpressApp Framework) — the .NET application framework for business apps with EF Core or XPO.

All skills target XAF v26.1+ (.NET 8+).

## Skills

| Skill | Covers | DevExpress Docs                                                       |
| --- | --- |---|
| [devexpress-xaf-business-model](skills/devexpress-xaf-business-model/) | Business model design: BaseObject, entity relationships, DbContext registration, data annotations, XAF attributes, seed data in Updater, EF Core migrations | [Overview](https://docs.devexpress.com/eXpressAppFramework/113461/business-model-design-orm) |
| [devexpress-xaf-business-logic](skills/devexpress-xaf-business-logic/) | CRUD operations & business logic: IObjectSpace, CreateObject, FindObject, CommitChanges, ObjectSpace events, IXafEntityObject lifecycle, NonPersistentObjectSpace | [Overview](https://docs.devexpress.com/eXpressAppFramework/113711/data-manipulation-and-business-logic) |
| [devexpress-xaf-business-logic-xpo](skills/devexpress-xaf-business-logic-xpo/) | XPO-specific business logic (sub-skill): Session, UnitOfWork, XPCollection, XPQuery, XPView, NestedUnitOfWork, SetPropertyValue, AfterConstruction/OnSaving overrides | [Overview](https://docs.devexpress.com/XPO/1998/express-persistent-objects) |
| [devexpress-xaf-controllers](skills/devexpress-xaf-controllers/) | Controllers & Actions: ViewController, WindowController, SimpleAction, SingleChoiceAction, PopupWindowShowAction, ParametrizedAction, controller lifecycle, ActionAttribute | [Overview](https://docs.devexpress.com/eXpressAppFramework/112621/ui-construction/controllers-and-actions) |
| [devexpress-xaf-views](skills/devexpress-xaf-views/) | Views & navigation: ListView, DetailView, DashboardView, ShowViewParameters, data access modes, edit modes, layout customization, non-persistent object views | [Overview](https://docs.devexpress.com/eXpressAppFramework/112611/ui-construction/views) |
| [devexpress-xaf-editors](skills/devexpress-xaf-editors/) | Editors: Property Editors, List Editors, View Items, data-type-to-editor mapping, CustomizeViewItemControl, custom Property/List Editors, BlazorControlViewItem | [Overview](https://docs.devexpress.com/eXpressAppFramework/113014/ui-construction/view-items-and-property-editors) |
| [devexpress-xaf-filtering](skills/devexpress-xaf-filtering/) | Filtering: CriteriaOperator syntax, IObjectSpace filtering, CollectionSource.Criteria, ListViewFilterAttribute, Find Panel, Filter Builder, ICustomFunctionOperator, lookup filtering | [Overview](https://docs.devexpress.com/eXpressAppFramework/112988/filtering) |
| [devexpress-xaf-filtering-xpo](skills/devexpress-xaf-filtering-xpo/) | XPO-specific filtering (sub-skill): XPCollection criteria, XPQuery LINQ, XPView, BinaryOperator/GroupOperator construction, Session.FindObject, server-mode data sources | [Overview](https://docs.devexpress.com/XPO/2034/query-and-shape-data) |
| [devexpress-xaf-appearance](skills/devexpress-xaf-appearance/) | Conditional Appearance: AppearanceAttribute, FontColor/BackColor/FontStyle/Enabled/Visibility, AppearanceItemType, criteria-based and method-based rules, AppearanceController events | [Overview](https://docs.devexpress.com/eXpressAppFramework/113286/conditional-appearance) |
| [devexpress-xaf-validation](skills/devexpress-xaf-validation/) | Validation: RuleRequiredField, RuleCriteria, RuleRange, RuleRegularExpression, RuleUniqueValue, DefaultContexts, soft validation, programmatic validation, custom rules | [Overview](https://docs.devexpress.com/eXpressAppFramework/113008/validation-module) |
| [devexpress-xaf-security](skills/devexpress-xaf-security/) | Security System: authentication (password, Windows, OAuth2), authorization (type/object/member/navigation permissions), Permission Policy, Audit Trail, security tiers | [Overview](https://docs.devexpress.com/eXpressAppFramework/113366/data-security-and-safety/security-system) |
| [devexpress-xaf-reports](skills/devexpress-xaf-reports/) | Reports V2 & data export: AddReports, CollectionDataSource, predefined reports, in-place reports, report preview from code, export to CSV/XLS/XLSX/PDF, Report Designer/Viewer | [Overview](https://docs.devexpress.com/eXpressAppFramework/113591/shape-export-print-data/reports) |
| [devexpress-xaf-performance](skills/devexpress-xaf-performance/) | Performance optimization: data access modes, EF Core eager loading, XPO delayed loading, N+1 detection, database indexing, model cache, Blazor tab management, SQL profiling | - |

## Composite Skills

Some skills follow a base + sub-skill pattern for ORM-specific code:

- **devexpress-xaf-business-logic** (base, ORM-agnostic) + **devexpress-xaf-business-logic-xpo** (XPO-specific)
- **devexpress-xaf-filtering** (base, ORM-agnostic) + **devexpress-xaf-filtering-xpo** (XPO-specific)

When the target project uses XPO, load both the base skill and the corresponding XPO sub-skill.
