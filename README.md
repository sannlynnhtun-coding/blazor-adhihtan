# Adhihtan — .NET 10 Blazor WebAssembly PWA

Adhihtan is a **standalone .NET 10 Blazor WebAssembly** progressive web app for managing daily vows and prayer-bead counting sessions. It works without a constant internet connection, requires no ASP.NET Core server project, and can be deployed directly to a static hosting provider such as Vercel.

## Features

- Local-only use with no login or server account required
- Built-in programs for Koe Naw Win, Khanti Ceti, One Thousand Virtues, the three-month Buddhist Lent, and custom one-day prayer-bead sessions
- 197 vow schedules with detailed guidance
- 13 prayer-bead counter styles, with sound, vibration, reset confirmation, and screen wake lock support
- Light, dark, and high-contrast themes, seasonal backgrounds, and Unicode/Zawgyi conversion based on Rabbit rules
- Automatic IndexedDB storage for programs, counts, and session history
- JSON backup export and import
- Full offline access to the app, WebAssembly runtime, images, audio, and data after the first online visit

## Run locally

```powershell
dotnet run --project .\Adhihtan\Adhihtan.csproj
```

Open the development URL shown in the console. The service worker does not cache files in development mode. To test the complete offline and PWA experience, use a Release build.

## Publish for static hosting

```powershell
dotnet publish .\Adhihtan\Adhihtan.csproj -c Release -o .\artifacts\publish
```

Deploy the files from `artifacts/publish/wwwroot` to your static host. Configure the host to rewrite or fall back all client-side routes to `/index.html`. Production deployments must use HTTPS to support PWA installation and service workers.

## Deploy to Vercel

The `vercel.json` file in the repository root automatically:

- Installs the .NET 10 SDK into the local `.dotnet` directory on the Vercel Linux build image
- Publishes the `Adhihtan` project in Release mode
- Deploys `artifacts/vercel/wwwroot` as the static output directory
- Rewrites all Blazor client-side routes to `/index.html`
- Revalidates the service worker while applying long-term caching to fingerprinted framework files

Import the repository from the Vercel dashboard and deploy it. You do not need to configure an additional framework preset, build command, or output directory in the project settings.

If the Vercel CLI is installed, you can also deploy from the repository root:

```powershell
vercel --prod
```

## Rebuild the content data

The `tools/recover-hermes-data.mjs` script reproducibly rebuilds category and schedule data from the source modules. Its current output is stored in `Adhihtan/wwwroot/data/recovered-content.json`.
