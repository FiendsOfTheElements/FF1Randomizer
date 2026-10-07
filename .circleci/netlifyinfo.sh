#!/bin/bash
set -e
set -x


config=$(jq -r ".branchConfig | map(select(if .branch == \"default\" then true elif .branch == \"${CIRCLE_BRANCH}\" then true else false end)) | .[0]" .circleci/configs/config.json)
branch=$(echo "$config" | jq -r ".branch")
netlifyID=$(echo "$config" | jq -r ".netlifyID")
deployPreview=$(echo "$config" | jq -r ".deployPreview")


curl --location --request GET 'https://api.netlify.com/api/v1/sites/${netlifyID}/deploys' \
    		       --header "Authorization: Bearer ${NETLIFY_AUTH_TOKEN}"

