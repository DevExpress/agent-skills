# AI Agent Skills for DevExpress XAF

AI agent skills for [DevExpress XAF](https://www.devexpress.com/products/net/application_framework/) (Cross-Platform .NET App UI). XAF is an ORM-based .NET Application Framework designed to help you deliver line-of-business apps in the shortest possible time (low-code, RAD). XAF seamlessly integrates award-winning DevExpress UI controls and ORM libraries (Entity Framework Core or XPO) so you can create feature-rich and highly interactive Windows Forms, ASP.NET Core Blazor, and Web API Service apps with ease.

--- 

## Prerequisites

- .NET 8+
- DevExpress XAF v26.1+
- [DevExpress Universal subscription license](https://www.devexpress.com/buy/winforms-wpf-blazor-asp-net-maui/)

---

## Included AI Agent Skills

| Skill | Capabilities | DevExpress Docs |
| --- | --- |---|
| [devexpress-xaf-business-model](skills/devexpress-xaf-business-model/) | Business Model Design: BaseObject, entity relationships, DbContext registration, data annotations, XAF attributes, calculated fields, PersistentAliasAttribute, seed data in Updater, deferred/soft deletion, optimistic locking | [Overview](https://docs.devexpress.com/eXpressAppFramework/113461/business-model-design-orm) |
| [devexpress-xaf-business-logic](skills/devexpress-xaf-business-logic/) | CRUD Operations & Business Logic: IObjectSpace, CreateObject, GetObjectsQuery, GetObjects, FindObject, FirstOrDefault, CommitChanges, DeleteObject, ObjectSpace events, IXafEntityObject methods and lifecycle, NonPersistentObjectSpace, Direct SQL | [Overview](https://docs.devexpress.com/eXpressAppFramework/113711/data-manipulation-and-business-logic) |
| [devexpress-xaf-business-logic-xpo](skills/devexpress-xaf-business-logic-xpo/) | XPO-specific Business Logic (sub-skill): Session, UnitOfWork, XPCollection, XPQuery, XPView, NestedUnitOfWork, SetPropertyValue, AfterConstruction/OnSaving overrides, IsSaving/IsLoading flags | [Overview](https://docs.devexpress.com/XPO/1998/express-persistent-objects) |
| [devexpress-xaf-controllers](devexpress-xaf-controllers/) | Controllers & Actions: ViewController, WindowController, SimpleAction, SingleChoiceAction, PopupWindowShowAction, ParametrizedAction, controller lifecycle, ActionAttribute, TargetViewId, TargetViewType | [Overview](https://docs.devexpress.com/eXpressAppFramework/112621/ui-construction/controllers-and-actions) |
| [devexpress-xaf-views](skills/devexpress-xaf-views/) | Views & Navigation: ListView, DetailView, DashboardView, ShowViewParameters, data access modes, edit modes, layout customization, non-persistent object views | [Overview](https://docs.devexpress.com/eXpressAppFramework/112611/ui-construction/views) |
| [devexpress-xaf-editors](skills/devexpress-xaf-editors/) | Editors: Property Editors, List Editors, View Items, .NET data type to Property Editor mapping, access editor controls, CustomizeViewItemControl, OnViewControlsCreated, custom Property/List Editors/View Items | [Overview](https://docs.devexpress.com/eXpressAppFramework/113014/ui-construction/view-items-and-property-editors) |
| [devexpress-xaf-filtering](skills/devexpress-xaf-filtering/) | Filtering: CriteriaOperator syntax, IObjectSpace filtering, CollectionSource.Criteria, ListViewFilterAttribute, Find Panel, Filter Builder, ICustomFunctionOperator, lookup filtering | [Overview](https://docs.devexpress.com/eXpressAppFramework/112988/filtering) |
| [devexpress-xaf-filtering-xpo](skills/devexpress-xaf-filtering-xpo/) | XPO-specific Filtering (sub-skill): XPCollection criteria, XPQuery LINQ, XPView, Session.FindObject/GetObjectByKey/`Query<T>`, server-mode data sources | [Overview](https://docs.devexpress.com/XPO/2034/query-and-shape-data) |
| [devexpress-xaf-appearance](skills/devexpress-xaf-appearance/) | Conditional Appearance: AppearanceAttribute, FontColor/BackColor/FontStyle/Enabled/Visibility, AppearanceItemType, criteria-based and method-based rules, AppearanceController events | [Overview](https://docs.devexpress.com/eXpressAppFramework/113286/conditional-appearance) |
| [devexpress-xaf-validation](skills/devexpress-xaf-validation/) | Validation: RuleRequiredField, RuleCriteria, RuleRange, RuleRegularExpression, RuleUniqueValue, DefaultContexts, soft validation, programmatic validation, custom rules | [Overview](https://docs.devexpress.com/eXpressAppFramework/113008/validation-module) |
| [devexpress-xaf-security](skills/devexpress-xaf-security/) | Security System: authentication (password, Windows, OAuth2), user logins, authorization (type/object/member/navigation permissions), roles and access rights, CurrentUserID, ApplicationUser/ISecurityUserWithRoles/ISecurityProvider/IsGrantedExtensions, Permission Policy, security tiers | [Overview](https://docs.devexpress.com/eXpressAppFramework/113366/data-security-and-safety/security-system) |
| [devexpress-xaf-reports](skills/devexpress-xaf-reports/) | Reports V2 & Data Export: AddReports, CollectionDataSource/ViewDataSource, predefined reports, in-place reports, report preview from code, export to CSV/XLS/XLSX/PDF, Report Designer/Viewer | [Overview](https://docs.devexpress.com/eXpressAppFramework/113591/shape-export-print-data/reports) |
| [devexpress-xaf-performance](skills/devexpress-xaf-performance/) | Performance Optimization: server-mode data sources, data access modes, EF Core eager/lazy/delayed loading, XPO delayed loading, calculated fields, PersistentAliasAttribute, N+1 Select Problem, database indexing, SQL query profiling | - |

---

## Composite Skills

Some skills follow a composite pattern (base skill + ORM-specific skill):

- **devexpress-xaf-business-logic** (base, ORM-agnostic) + **devexpress-xaf-business-logic-xpo** (XPO-specific)
- **devexpress-xaf-filtering** (base, ORM-agnostic) + **devexpress-xaf-filtering-xpo** (XPO-specific)

If your project uses XPO, load both the base skill and the corresponding XPO sub-skill.

---

## Skill Folder Content

Each skill is self-contained and follows the same structure:

```
devexpress-xaf-<name>/
├── SKILL.md      -- YAML frontmatter (activators, prerequisites), navigation guide
└── references/   -- scenario-focused deep dives
```

---

For agent-specific and IDE-specific setup instructions, see the [repository README](../../README.md).
