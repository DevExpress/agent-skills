# Lifetimes

WPF has no request scope; use singleton and transient. Do not create scopes unless the project already has a unit-of-work concept that needs one.

| Kind of object | Lifetime | Reason |
|---|---|---|
| Stateless service (repository over an in-memory list or a file, clock, formatter) | Singleton | One instance is enough; cheapest. A wrapper around a typed `HttpClient` follows the `HttpClient` row instead |
| Shared application state (selection, current user, settings, caches, event hubs, message bus) | **Singleton** | Every consumer must see the same instance — this replaces `Type.Instance` / `Type.Current` |
| Former static singleton (`X.Instance`) | Singleton | Keeps today's behaviour |
| View model | Transient | One per view instance; a view created twice must not share state by accident |
| View model that today is intentionally shared (same instance given to several views) | Singleton | Keep the existing sharing |
| Window, view, UserControl registered for constructor injection | Transient | WPF elements cannot be reused across visual trees |
| `DbContext` | Register `AddDbContextFactory` and create a context per operation | A long-lived context in a singleton or a window-lifetime view model grows and goes stale |
| `HttpClient` | `AddHttpClient` — package `Microsoft.Extensions.Http` (not part of `Microsoft.Extensions.Hosting`); inject the typed client into transient consumers, or inject `IHttpClientFactory` into a singleton and create a client per call | Avoids socket exhaustion; a typed client held by a singleton keeps one handler for good and misses DNS changes |
| Unattached DevExpress service (message box) | Singleton | See `dx-services.md` |

## Captive dependencies

A singleton must not take a transient view model, a view, a `DbContext`, or anything per-window in its constructor. If it needs one, inject a factory (`Func<T>` registered explicitly, or `IServiceScopeFactory` only where scopes are already used).

`ValidateOnBuild = true` / `ValidateScopes = true` on `BuildServiceProvider` (or `builder.Services` validation defaults in the Host in Development) make missing registrations fail at startup instead of at first use.

## When the code cannot tell

Default to the table and state the choice in the plan. Ask only when a service holds state and it is unclear whether consumers should share it.
