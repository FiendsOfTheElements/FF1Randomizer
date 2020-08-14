#!/bin/sh
config=$(jq -r ".branchConfig | map(select(if .branch == \"default\" then true elif .branch == \"${CIRCLE_BRANCH}\" then true else false end)) | .[0]" .circleci/configs/config.json)
echo "$config" | cat
netlifyID=$(echo "$config" | jq -r ".netlifyID")
deployPreview=$(echo "$config" | jq -r ".deployPreview")
if "$deployPreview"; then
    # netlify deploy --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --site="$netlifyID" | grep "Website Draft URL" | cat
    # netlify deploy --dir=FF1Blazorizer/output/wwwroot --site=4ef4838f-158d-4929-bd24-d516bec18708 --json | jq -r ".deploy_url" | cat

    GH_USER=Testing
    GH_API=9b7287752289cc259bdf8f720d5a416f3c9984d5
    pr_response=$(curl --location --request GET "https://api.github.com/repos/$CIRCLE_PROJECT_USERNAME/$CIRCLE_PROJECT_REPONAME/pulls?head=$CIRCLE_PROJECT_USERNAME:$CIRCLE_BRANCH&state=open" \
        -u $GH_USER:$GH_API)

    if [ $(echo $pr_response | jq length) -eq 0 ]; then
        echo "No PR found to update"
    else
        pr_comment_url=$(echo $pr_response | jq -r ".[]._links.comments.href")
    fi

    curl --location --request POST "$pr_comment_url" \
        -u $GH_USER:$GH_API \
        --header 'Content-Type: application/json' \
        --data-raw '{
       "body": "This would be the data to add"
      }'

else
    # netlify deploy --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --prod --site="$netlifyID"
fi
