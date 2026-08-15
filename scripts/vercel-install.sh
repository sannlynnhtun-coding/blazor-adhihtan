#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOTNET_DIR="$PROJECT_ROOT/.dotnet"
INSTALL_SCRIPT="$PROJECT_ROOT/.dotnet-install.sh"

if [[ -x "$DOTNET_DIR/dotnet" ]]; then
    "$DOTNET_DIR/dotnet" --version
    exit 0
fi

mkdir -p "$DOTNET_DIR"
curl --fail --silent --show-error --location \
    https://dot.net/v1/dotnet-install.sh \
    --output "$INSTALL_SCRIPT"
bash "$INSTALL_SCRIPT" --channel 10.0 --quality GA --install-dir "$DOTNET_DIR"
"$DOTNET_DIR/dotnet" --version
