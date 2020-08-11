#!/bin/sh
set -o errexit
set -o verbose

sed -i "s/SHA/${CIRCLE_SHA1}/" FF1Lib/FFRVersion.cs
sed -i "s/BRANCH/${CIRCLE_BRANCH}/" FF1Lib/FFRVersion.cs

cd FF1Blazorizer

config=jq ".branchConfig | map(select(.branch == ${CIRCLE_BRANCH})) | .[0]"
echo config

if [ "${CIRCLE_BRANCH}" = "master" ]; then
    sed -i 's/NAME_LONG/FFRandomizer/g' wwwroot/manifest.published.json
    sed -i 's/NAME_SHORT/FFR/g' wwwroot/manifest.published.json
    color=$(grep "DARK_BACKGROUND_COLOR: #.*" wwwroot/css/main.css | grep -o "#[0-9a-fA-F]*")
    sed -i "s/DARK_BACKGROUND_COLOR/$color/g" wwwroot/manifest.published.json
    sed -i "s/DARK_BACKGROUND_COLOR/$color/g" wwwroot/index.html
    sed -i 's/SITE_ICON_COLOR/normal/g' wwwroot/manifest.published.json
    sed -i 's/SITE_ICON_COLOR/normal/g' wwwroot/index.html
    mv -f wwwroot/manifest.published.json wwwroot/manifest.json
    dotnet publish -c Release -o output
elif [ "${CIRCLE_BRANCH}" = "dev" ]; then
    sed -i 's/NAME_LONG/FFRandomizer - beta/g' wwwroot/manifest.published.json
    sed -i 's/NAME_SHORT/FFR β/g' wwwroot/manifest.published.json
    color=$(grep "DARK_BACKGROUND_COLOR: #.*" wwwroot/css/beta.css | grep -o "#[0-9a-fA-F]*")
    sed -i "s/DARK_BACKGROUND_COLOR/$color/g" wwwroot/manifest.published.json
    sed -i "s/DARK_BACKGROUND_COLOR/$color/g" wwwroot/index.html
    sed -i 's/main.css/beta.css/g' wwwroot/css/site.css
    sed -i 's/SITE_ICON_COLOR/cyan/g' wwwroot/manifest.published.json
    sed -i 's/SITE_ICON_COLOR/cyan/g' wwwroot/index.html
    mv -f wwwroot/manifest.published.json wwwroot/manifest.json
    dotnet publish -c Debug -o output
elif [ "${CIRCLE_BRANCH}" = "alpha" ]; then
    sed -i 's/NAME_LONG/FFRandomizer - alpha/g' wwwroot/manifest.published.json
    sed -i 's/NAME_SHORT/FFR α/g' wwwroot/manifest.published.json
    color=$(grep "DARK_BACKGROUND_COLOR: #.*" wwwroot/css/alpha.css | grep -o "#[0-9a-fA-F]*")
    sed -i "s/DARK_BACKGROUND_COLOR/$color/g" wwwroot/manifest.published.json
    sed -i "s/DARK_BACKGROUND_COLOR/$color/g" wwwroot/index.html
    sed -i 's/main.css/alpha.css/g' wwwroot/css/site.css
    sed -i 's/SITE_ICON_COLOR/red/g' wwwroot/manifest.published.json
    sed -i 's/SITE_ICON_COLOR/red/g' wwwroot/index.html
    mv -f wwwroot/manifest.published.json wwwroot/manifest.json
    dotnet publish -c Debug -o output
else
    dotnet publish -c Debug -o output
fi
