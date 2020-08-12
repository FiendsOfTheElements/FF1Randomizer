#!/bin/bash
set -o errexit
set -o verbose

sed -i "s/SHA/${CIRCLE_SHA1}/" FF1Lib/FFRVersion.cs
sed -i "s/BRANCH/${CIRCLE_BRANCH}/" FF1Lib/FFRVersion.cs

cd FF1Blazorizer

config=$(jq -r ".branchConfig | map(select(if .branch == \"default\" then true elif .branch == \"master\" then true else false end)) | .[0]" ../.circleci/configs/config.json)
cat <<<$config
longName=$(jq ".longName" <<<"$config")
shortName=$(jq ".shortName" <<<"$config")
cssFile=$(jq ".cssFile" <<<"$config")
themeColor=$(jq ".themeColor" <<<"$config")
siteIcon=$(jq ".siteIcon" <<<"$config")
releaseBuild=$(jq ".releaseBuild" <<<"$config")

sed -i 's/NAME_LONG/FFRandomizer/g' wwwroot/manifest.published.json
sed -i 's/NAME_SHORT/FFR/g' wwwroot/manifest.published.json
sed -i "s/DARK_BACKGROUND_COLOR/$color/g" wwwroot/manifest.published.json
sed -i "s/DARK_BACKGROUND_COLOR/$color/g" wwwroot/index.html
sed -i 's/SITE_ICON_COLOR/normal/g' wwwroot/manifest.published.json
sed -i 's/SITE_ICON_COLOR/normal/g' wwwroot/index.html
mv -f wwwroot/manifest.published.json wwwroot/manifest.json
dotnet publish -c Release -o output
