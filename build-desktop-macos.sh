#!/bin/bash
# Wrapper script to build Desktop project on macOS
# This script forwards to the Desktop project's build script

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DESKTOP_SCRIPT="$SCRIPT_DIR/src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop/build-scripts/build-local-macos.sh"

echo "=== Building Desktop Project ==="
echo "Forwarding to: $DESKTOP_SCRIPT"
echo ""

if [ -f "$DESKTOP_SCRIPT" ]; then
    chmod +x "$DESKTOP_SCRIPT"
    "$DESKTOP_SCRIPT" "$@"
else
    echo "Error: Desktop build script not found at: $DESKTOP_SCRIPT"
    exit 1
fi

