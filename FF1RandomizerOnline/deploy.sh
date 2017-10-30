dotnet publish -c Release
docker build -t ff1randomizeronline:latest .
docker stop ff1ro
docker rm ff1ro
docker run -d --name ff1ro --network web-internal ff1randomizeronline
