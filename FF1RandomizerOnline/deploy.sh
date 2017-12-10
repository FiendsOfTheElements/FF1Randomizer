#!/bin/bash
branch_name=$(git symbolic-ref --short -q HEAD)
if [ $branch_name == 'dev' ]; then
	dotnet publish -c Debug
	docker build -t ff1randomizeronline:beta -f Dockerfile.beta .
	docker stop ff1robeta
	docker rm ff1robeta
	docker run -d --name ff1robeta --network web-internal ff1randomizeronline:beta
elif [ $branch_name == 'master' ]; then
	dotnet publish -c Release
	docker build -t ff1randomizeronline:latest .
	docker stop ff1ro
	docker rm ff1ro
	docker run -d --name ff1ro --network web-internal ff1randomizeronline
else
	echo "Unknown branch.  Checkout master or dev to deploy."
fi
