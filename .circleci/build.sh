#!/bin/sh
set -o errexit
set -x

sed -i "s/SHA/${CIRCLE_SHA1}/" FF1Lib/FFRVersion.cs
sed -i "s|BRANCH|${CIRCLE_BRANCH}|" FF1Lib/FFRVersion.cs

cd FF1Blazorizer

config=$(jq -r ".branchConfig | map(select(if .branch == \"default\" then true elif .branch == \"${CIRCLE_BRANCH}\" then true else false end)) | .[0]" ../.circleci/configs/config.json)
echo "$config" | cat
longName=$(echo "$config" | jq -r ".longName")
shortName=$(echo "$config" | jq -r ".shortName")
cssFile=$(echo "$config" | jq -r ".cssFile")
themeColor=$(echo "$config" | jq -r ".themeColor")
siteIcon=$(echo "$config" | jq -r ".siteIcon")
releaseBuild=$(echo "$config" | jq -r ".releaseBuild")

sed -i "s/NAME_LONG/${longName}/g" wwwroot/manifest.published.json
sed -i "s/NAME_SHORT/$shortName/g" wwwroot/manifest.published.json
sed -i "s/DARK_BACKGROUND_COLOR/$themeColor/g" wwwroot/manifest.published.json
sed -i "s/DARK_BACKGROUND_COLOR/$themeColor/g" wwwroot/index.html
sed -i "s/main.css/$cssFile/g" wwwroot/css/site.css 
sed -i "s/SITE_ICON_COLOR/$siteIcon/g" wwwroot/manifest.published.json
sed -i "s/SITE_ICON_COLOR/$siteIcon/g" wwwroot/index.html
mv -f wwwroot/manifest.published.json wwwroot/manifest.json
if "$releaseBuild"; then
    dotnet publish -c Release -o output
else
    dotnet publish -c Debug -o output
fi
