#!/bin/zsh

# Project path (adjust if needed)
PROJECT_PATH="../"

# Output base folder
OUTPUT_DIR="../publish"

# Platforms to build
declare -A RID_MAP
RID_MAP=( 
  ["win-x64"]="Windows x64"
  ["linux-x64"]="Linux x64"
  ["osx-x64"]="macOS Intel"
  ["osx-arm64"]="macOS Apple Silicon"
)

# Loop through each runtime identifier
for RID in ${(k)RID_MAP}; do
  echo "Publishing for ${RID_MAP[$RID]} ($RID)..."

  dotnet publish $PROJECT_PATH \
    -c Release \
    -r $RID \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:IncludeAllContentForSelfExtract=true \
    -o "$OUTPUT_DIR/$RID"

  if [[ $? -eq 0 ]]; then
    echo "✅ Published for $RID_MAP[$RID] successfully."
  else
    echo "❌ Failed to publish for $RID_MAP[$RID]."
  fi
done

echo "All builds finished. Output is in $OUTPUT_DIR/"
