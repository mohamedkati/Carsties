FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

COPY AuctionsMicroServices.sln AuctionsMicroServices.sln
COPY AuctionService/AuctionService.csproj AuctionService/AuctionService.csproj
COPY SearchService/SearchService.csproj SearchService/SearchService.csproj
COPY Contracts/Contracts.csproj Contracts/Contracts.csproj
COPY GatewayService/GatewayService.csproj GatewayService/GatewayService.csproj
COPY IdentityService/IdentityService.csproj IdentityService/IdentityService.csproj
COPY BidService/BidService.csproj BidService/BidService.csproj
COPY NotificationService/NotificationService.csproj NotificationService/NotificationService.csproj 

RUN dotnet restore AuctionsMicroServices.sln

WORKDIR /app
COPY BidService BidService
COPY Contracts Contracts


WORKDIR /app/BidService
RUN dotnet publish "BidService.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BidService.dll"]
