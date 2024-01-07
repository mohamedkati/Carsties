FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

COPY AuctionsMicroServices.sln AuctionsMicroServices.sln
COPY AuctionService/AuctionService.csproj AuctionService/AuctionService.csproj 
COPY BidService/BidService.csproj BidService/BidService.csproj 
COPY Contracts/Contracts.csproj Contracts/Contracts.csproj 
COPY GatewayService/GatewayService.csproj GatewayService/GatewayService.csproj 
COPY IdentityService/IdentityService.csproj IdentityService/IdentityService.csproj 
COPY NotificationService/NotificationService.csproj NotificationService/NotificationService.csproj 
COPY SearchService/SearchService.csproj SearchService/SearchService.csproj 

RUN dotnet restore AuctionsMicroServices.sln

COPY NotificationService NotificationService
COPY Contracts Contracts

WORKDIR /app/NotificationService

#RUN dotnet build NotificationService.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
