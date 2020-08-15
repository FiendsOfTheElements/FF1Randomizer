#!/bin/sh
set -o errexit
set -o verbose

config=$(jq -r ".branchConfig | map(select(if .branch == \"default\" then true elif .branch == \"${CIRCLE_BRANCH}\" then true else false end)) | .[0]" .circleci/configs/config.json)
echo "$config" | cat
netlifyID=$(echo "$config" | jq -r ".netlifyID")
deployPreview=$(echo "$config" | jq -r ".deployPreview")
if "$deployPreview"; then
    url=$(netlify deploy --json --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --site="$netlifyID" | jq -r ".deploy_url")

    GH_USER=FFR_Build_And_Deploy
    pr_response=$(curl --location --request GET "https://api.github.com/repos/$CIRCLE_PROJECT_USERNAME/$CIRCLE_PROJECT_REPONAME/pulls?head=$CIRCLE_PROJECT_USERNAME:$CIRCLE_BRANCH&state=open" -u $GH_USER:"$GH_API")

    if [ "$(echo "$pr_response" | jq length)" -eq 0 ]; then
        echo "No PR found to update"
    else
        pr_comment_url=$(echo "$pr_response" | jq -r ".[]._links.comments.href")
    fi
    post_data='{"body": "Automatic deployment: '$url'"}'
    curl -X POST -H "Accept: application/vnd.github.v3+json" -H "Content-Type:application/json" "$pr_comment_url" -u $GH_USER:"$GH_API" -d "$post_data"

else
    echo "nothing"
    # netlify deploy --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --prod --site="$netlifyID"
fi
