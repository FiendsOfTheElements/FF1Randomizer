#!/bin/bash
set -o errexit
set -o verbose

sed -i "s/SHA/${CIRCLE_SHA1}/" FF1Lib/FFRVersion.cs
sed -i "s/BRANCH/${CIRCLE_BRANCH}/" FF1Lib/FFRVersion.cs

cd FF1Blazorizer

ls ../.circleci/ -a
config=$(jq -r ".branchConfig | map(select(if .branch == \"default\" then true elif .branch == ${CIRCLE_BRANCH} then true else false end)) | .[0]" ../.circleci/configs/config.json)
cat <<< "$config"
longName=$(jq -r ".longName" <<<"$config")
shortName=$(jq -r ".shortName" <<<"$config")
# cssFile=$(jq -r ".cssFile" <<<"$config")
themeColor=$(jq -r ".themeColor" <<<"$config")
siteIcon=$(jq -r ".siteIcon" <<<"$config")
releaseBuild=$(jq -r ".releaseBuild" <<<"$config")

sed -i "s/NAME_LONG/${longName}/g" wwwroot/manifest.published.json
sed -i "s/NAME_SHORT/$shortName/g" wwwroot/manifest.published.json
sed -i "s/DARK_BACKGROUND_COLOR/$themeColor/g" wwwroot/manifest.published.json
sed -i "s/DARK_BACKGROUND_COLOR/$themeColor/g" wwwroot/index.html
sed -i "s/SITE_ICON_COLOR/$siteIcon/g" wwwroot/manifest.published.json
sed -i "s/SITE_ICON_COLOR/$siteIcon/g" wwwroot/index.html
mv -f wwwroot/manifest.published.json wwwroot/manifest.json
dotnet publish -c Release -o output
