#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT="$PROJECT_ROOT/Adhihtan/Adhihtan.csproj"
OUTPUT="$PROJECT_ROOT/artifacts/vercel"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

if [[ -x "$PROJECT_ROOT/.dotnet/dotnet" ]]; then
    DOTNET="$PROJECT_ROOT/.dotnet/dotnet"
elif command -v dotnet >/dev/null 2>&1; then
    DOTNET="$(command -v dotnet)"
else
    echo ".NET SDK is missing. Run scripts/vercel-install.sh first." >&2
    exit 1
fi

"$DOTNET" restore "$PROJECT"
"$DOTNET" publish "$PROJECT" -c Release --no-restore -o "$OUTPUT"
test -f "$OUTPUT/wwwroot/index.html"
test -f "$OUTPUT/wwwroot/service-worker-assets.js"
