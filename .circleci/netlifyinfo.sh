#!/bin/bash
set -e


days=365


thispage=1
allsites=""

while true; do

    response=$(curl --location --request GET "https://api.netlify.com/api/v1/sites?page=${thispage}" \
    		       --header "Authorization: Bearer ${NETLIFY_AUTH_TOKEN}")
    
    if [ "$(echo "$response" | jq length)" -eq 0 ]; then
        break
    fi

    allsites+="${response}"
    ((thispage++))

done

mapfile -t site_ids < <(echo "$allsites" | \
             jq -s 'add' | \
             jq --argjson days "${days}" '[.[] | select(.custom_domain | startswith("beta-")) | 
                select((.created_at | sub("\\.[0-9]+"; "")) | fromdateiso8601 < (now - ($days * 86400))) ] |
                sort_by(.created_at) | reverse' | \
             jq -r '.[] | .id' )


for id in "${site_ids[@]}"; do
    echo ""
    echo "Deleting site ID ${id}"
    
done
    




