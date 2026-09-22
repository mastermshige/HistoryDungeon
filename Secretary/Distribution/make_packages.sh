#!/bin/bash
# Builds/ にあるビルド成果物から、配布用の zip（Mac・Windows）を作る。
# 使い方: プロジェクトのルートで  bash Secretary/Distribution/make_packages.sh
#
# Mac 版で行うこと（重要）:
#   Unity が作る Mac のアプリは、実行ファイル名に日本語（製品名）が入るため、
#   署名の検証（codesign --verify）に失敗して、他の Mac で「壊れている」と出る。
#   そこで、実行ファイル名を英字（HistoryDungeon）にし、Finder に出る名前は
#   CFBundleDisplayName で日本語のままにして、署名をやり直す。
set -euo pipefail
cd "$(dirname "$0")/../.."

VERSION="0.1.0"
DIST="Secretary/Distribution"
OUT="Builds"
APP_SRC="$OUT/Mac/HistoryDungeon.app"
WIN_SRC="$OUT/Windows/HistoryDungeon"
DISPLAY_NAME="歴史年号ダンジョン"
STAGE="$(mktemp -d)"
trap 'rm -rf "$STAGE"' EXIT

[ -d "$APP_SRC" ] || { echo "Mac のビルドがありません: $APP_SRC"; exit 1; }
[ -d "$WIN_SRC" ] || { echo "Windows のビルドがありません: $WIN_SRC"; exit 1; }

# ---------- Mac ----------
MAC_DIR="$STAGE/HistoryDungeon_Mac"
mkdir -p "$MAC_DIR"
ditto --norsrc --noextattr --noqtn "$APP_SRC" "$MAC_DIR/HistoryDungeon.app"
APP="$MAC_DIR/HistoryDungeon.app"
EXE_OLD="$(plutil -extract CFBundleExecutable raw "$APP/Contents/Info.plist")"
if [ "$EXE_OLD" != "HistoryDungeon" ]; then
  mv "$APP/Contents/MacOS/$EXE_OLD" "$APP/Contents/MacOS/HistoryDungeon"
  plutil -replace CFBundleExecutable -string HistoryDungeon "$APP/Contents/Info.plist"
fi
plutil -replace CFBundleDisplayName -string "$DISPLAY_NAME" "$APP/Contents/Info.plist"
# 内側のライブラリから順に署名し、最後にアプリ全体に署名する
find "$APP" -type f \( -name "*.dylib" -o -name "*.bundle" \) -print0 | xargs -0 -n1 codesign --force --sign - >/dev/null 2>&1
codesign --force --sign - "$APP" >/dev/null 2>&1
codesign --verify --deep --strict "$APP"
echo "Mac: 署名の検証 OK"
cp "$DIST/README.txt" "$MAC_DIR/"
cp -R "$DIST/Licenses" "$MAC_DIR/"
rm -f "$OUT/HistoryDungeon_Mac_${VERSION}.zip"
ditto -c -k --norsrc --keepParent "$MAC_DIR" "$OUT/HistoryDungeon_Mac_${VERSION}.zip"

# ---------- Windows ----------
WIN_DIR="$STAGE/HistoryDungeon"
mkdir -p "$WIN_DIR"
rsync -a --exclude "*BurstDebugInformation*" "$WIN_SRC/" "$WIN_DIR/"
cp "$DIST/README.txt" "$WIN_DIR/"
cp -R "$DIST/Licenses" "$WIN_DIR/"
rm -f "$OUT/HistoryDungeon_Windows_${VERSION}.zip"
(cd "$STAGE" && zip -qr "$OLDPWD/$OUT/HistoryDungeon_Windows_${VERSION}.zip" HistoryDungeon)

ls -lh "$OUT"/HistoryDungeon_*_"${VERSION}".zip
