#!/bin/sh 
if [ "${CIRCLE_BRANCH}" = "master" ]; then
    netlify deploy --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --prod --site=cd1fef70-df7f-44ab-b1c8-3239dae773ac
elif [ "${CIRCLE_BRANCH}" = "dev" ]; then
    netlify deploy --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --prod --site=b1e4219e-dfb6-4efa-924f-3136bbf3fa26
else
    netlify deploy --dir=/root/ff1randomizer/FF1Blazorizer/output/wwwroot --prod --site=61141991-cfa6-4a2e-ab6b-187b1611a2e4
fi
