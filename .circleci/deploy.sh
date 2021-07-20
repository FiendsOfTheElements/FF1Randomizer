#!/bin/sh
set -e
set -x


config=$(jq -r ".branchConfig | map(select(if .branch == \"default\" then true elif .branch == \"${CIRCLE_BRANCH}\" then true else false end)) | .[0]" .circleci/configs/config.json)
branch=$(echo "$config" | jq -r ".branch")
netlifyID=$(echo "$config" | jq -r ".netlifyID")
deployPreview=$(echo "$config" | jq -r ".deployPreview")


if "$deployPreview"; then
    deploy_response=$(netlify deploy --json --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --site="$netlifyID")
    url=$(echo "$deploy_response" | jq -r ".deploy_url")

    GH_USER=FFR_Build_And_Deploy
    pr_response=$(curl --location --request GET "https://api.github.com/repos/$CIRCLE_PROJECT_USERNAME/$CIRCLE_PROJECT_REPONAME/pulls?head=$CIRCLE_PROJECT_USERNAME:$CIRCLE_BRANCH&state=open" -u $GH_USER:"$GH_API")

    if [ "$(echo "$pr_response" | jq length)" -eq 0 ]; then
        echo "No PR found to update"
        exit 0
    else
        pr_comment_url=$(echo "$pr_response" | jq -r ".[]._links.comments.href")
    fi
    post_data='{"body": "Automatic deployment: '$url'"}'
    curl -X POST -H "Accept: application/vnd.github.v3+json" -H "Content-Type:application/json" "$pr_comment_url" -u $GH_USER:"$GH_API" -d "$post_data"
else
    if [ "$branch" = "master" ]; then
	version=$(grep " Version.*" /root/ff1randomizer/FF1Lib/FFRVersion.cs | grep -Eo "[0-9\.]+" | tr '.' '-')
    elif [ "$branch" = "dev" ]; then
	version=beta-$(echo "$CIRCLE_SHA1" | cut -c1-8)
    else
    	echo "Don't know what to do to deploy branch '$branch' expected 'master' or 'dev'"
    	exit 1
    fi

    siteExists=$(curl --location --request GET 'https://api.netlify.com/api/v1/dns_zones/finalfantasyrandomizer_com/dns_records' \
    		      --header "Authorization: Bearer ${NETLIFY_AUTH_TOKEN}" \
    		      --header 'Content-Type: application/json' | jq -r ".[].hostname" | grep -q "${version}" && echo true || echo false
    	      )

    if [ "${siteExists}" = true ]; then
    	echo "The version ${version} was found in the dns entries, make sure you increment the version in FF1Lib/FFRVersion.cs"
    	exit 1
    fi

    createdSite=$(curl --location --request POST 'https://api.netlify.com/api/v1/sites' \
    		       --header "Authorization: Bearer ${NETLIFY_AUTH_TOKEN}" \
    		       --header 'Content-Type: application/json' \
    		       --data-raw "{\"custom_domain\": \"${version}.finalfantasyrandomizer.com\", \"force_ssl\": \"true\"}")


    errors=$(echo "$createdSite" | jq ".errors")
    if [ "$errors" -ne "null" ]; then
    	echo "errors encountered while creating site:"
    	echo "$errors"
    	exit 2
    fi

    id=$(echo "$createdSite" | jq -r ".site_id")
    echo "$id"

    # Deploy the instanced site
    netlify deploy --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --prod --site="$id"

    # Copy over index.redirect.html, this will make the front page of
    # either the main site or the beta site to redirect to the
    # instanced site that we just deployed.
    cp /root/ff1randomizer/FF1Blazorizer/output/wwwroot/index.redirect.html /root/ff1randomizer/FF1Blazorizer/output/wwwroot/index.html
    sed -i "s/VERSION/${version}/" /root/ff1randomizer/FF1Blazorizer/output/wwwroot/index.html

    # Also record version into a simple file that we can fetch to find out the latest version.
    echo "${version}" > /root/ff1randomizer/FF1Blazorizer/output/wwwroot/version

    # Deploy with the redirect version of index.html
    netlify deploy --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --prod --site="$netlifyID"
fi
