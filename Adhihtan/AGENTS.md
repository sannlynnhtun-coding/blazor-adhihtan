# Adhihtan

| Setting | Value |
|---|---|
| Runtime | Standalone Blazor WebAssembly |
| Framework | .NET 10 |
| Hosting | Static files / Vercel |
| Offline | PWA service worker + IndexedDB |

## Architecture

- This is the only application project. There is no ASP.NET Core server project.
- All pages, components, services, and models run entirely in the browser.
- Static application content lives under `wwwroot` and user state lives in IndexedDB.
- Do not add server-only APIs, `HttpContext`, server file access, or online dependencies to core flows.

## Routing and deployment

- `App.razor` owns client-side routing.
- `wwwroot/index.html` is the static host page.
- Vercel rewrites non-file routes to `index.html` for SPA deep links.
- Release publish output is the `wwwroot` directory inside the publish folder.

## PWA

- `service-worker.js` is intentionally network-only during development.
- Release publish substitutes `service-worker.published.js` and generates `service-worker-assets.js`.
- Keep counting, categories, settings, backup, and restore functional offline.
