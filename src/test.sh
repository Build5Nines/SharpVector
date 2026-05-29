#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
RESULTS_DIR="$SCRIPT_DIR/TestResults/Coverage"
REPORT_DIR="$SCRIPT_DIR/coveragereport"

rm -rf "$RESULTS_DIR" "$REPORT_DIR"
mkdir -p "$RESULTS_DIR" "$REPORT_DIR"

cd "$SCRIPT_DIR"

echo "Running unit tests with code coverage..."
dotnet test SharpVector.sln \
  --configuration Debug \
  --results-directory "$RESULTS_DIR" \
  --logger "trx;LogFileName=test_results.trx" \
  --collect:"XPlat Code Coverage"

echo "Installing/updating ReportGenerator..."
if ! dotnet tool update --global dotnet-reportgenerator-globaltool >/dev/null 2>&1; then
  dotnet tool install --global dotnet-reportgenerator-globaltool >/dev/null 2>&1
fi

REPORTGENERATOR_CMD="${HOME}/.dotnet/tools/reportgenerator"

echo "Generating coverage report..."
"$REPORTGENERATOR_CMD" \
  -reports:"$RESULTS_DIR/**/coverage.cobertura.xml" \
  -targetdir:"$REPORT_DIR" \
  -reporttypes:"Html;MarkdownSummary;TextSummary"

echo
echo "Coverage report generated:"
echo "  HTML: $REPORT_DIR/index.html"
echo "  Markdown: $REPORT_DIR/Summary.md"
echo
cat "$REPORT_DIR/Summary.txt"
