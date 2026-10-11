#!/bin/bash
set -e

export TERM=${TERM:-xterm-256color}

while getopts "dp" opt; do
    case "${opt}" in
        d)
            DRYRUN="true"
            break
            ;;
        p)
            DRYRUN="false"
            ;;
        *)
            echo "Requires either -d (dry run) or -p (pruning mode).">&2
            exit 1
            ;;
    esac
done

shift $((OPTIND - 1))
DAYS=$1
MAXSITES=$2

if [[ "${DRYRUN}" == "true" ]]; then
	tput bold
    echo "Executing dry run of max ${MAXSITES} sites older than ${DAYS} days."
    tput sgr0
else
	tput bold
    echo "Pruning max ${MAXSITES} sites older than ${DAYS} days."
    tput sgr0
fi
echo ""

thispage=1
allsites=""

echo ""
tput bold
echo "Getting list of all sites..."
tput sgr0
echo ""
echo ""


while true; do

    response=$(curl --location --request GET "https://api.netlify.com/api/v1/sites?page=${thispage}" \
    		        --header "Authorization: Bearer ${NETLIFY_AUTH_TOKEN}")

    if [ "$(echo "$response" | jq 'length')" -eq 0 ]; then
        break
    fi

    allsites+="${response}"
    ((thispage++))

done

echo ""
tput bold
echo "... done."
tput sgr0
echo ""
echo ""
echo ""
echo ""
echo ""
echo ""



mapfile -t site_ids < <(echo "$allsites" | \
             jq -s 'add' | \
             jq --argjson days "${DAYS}" --argjson maxprune "${MAXSITES}" '[.[] | select(.custom_domain | startswith("beta-")) |
                select((.created_at | sub("\\.[0-9]+"; "")) | fromdateiso8601 < (now - ($days * 86400))) ] |
                sort_by(.created_at) | .[:$maxprune] | reverse' | \
             jq -r '.[] | .id' )




for id in "${site_ids[@]}"; do
    response=$(curl --location --request GET "https://api.netlify.com/api/v1/sites/${id}" \
                    --header "Authorization: Bearer ${NETLIFY_AUTH_TOKEN}")
    datetime=$(echo "$response" | jq '.created_at')
    url=$(echo "$response" | jq '.url')
    tput bold
    echo "URL: ${url}"
    echo "Site ID: ${id}"
    echo "Date Created: ${datetime}"
    if [[ "${DRYRUN}" == "false" ]]; then
        echo "Deleting..."
        tput sgr0
        curl --location --request DELETE "https://api.netlify.com/api/v1/sites/${id}" \
             --header "Authorization: Bearer ${NETLIFY_AUTH_TOKEN}"
    else
        echo "(Dry-run)..."
        tput sgr0
    fi
    echo ""
    
    sleep 2

done



