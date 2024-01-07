FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
# define the current directory 
WORKDIR /app
EXPOSE 80
# copying all the dependencies of the project (.sln)
COPY AuctionsMicroServices.sln AuctionsMicroServices.sln
COPY AuctionService/AuctionService.csproj AuctionService/AuctionService.csproj
COPY SearchService/SearchService.csproj SearchService/SearchService.csproj
COPY IdentityService/IdentityService.csproj IdentityService/IdentityService.csproj
COPY Contracts/Contracts.csproj Contracts/Contracts.csproj
COPY GatewayService/GatewayService.csproj GatewayService/GatewayService.csproj
COPY BidService/BidService.csproj BidService/BidService.csproj
COPY NotificationService/NotificationService.csproj NotificationService/NotificationService.csproj 

# restore all the dependencies packages
RUN dotnet restore AuctionsMicroServices.sln

# copying all {specified_directoty} to /app/{specified_directory}
COPY IdentityService IdentityService

# enter to identityService directory
WORKDIR "/app/IdentityService"

# publish the app using release mode
RUN dotnet publish "IdentityService.csproj" -c Release -o /app/publish

# run the app using entry point and dot net runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","IdentityService.dll"]
