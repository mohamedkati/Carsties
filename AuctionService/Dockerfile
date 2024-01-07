

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
WORKDIR /app
EXPOSE 80
COPY AuctionsMicroServices.sln AuctionsMicroServices.sln
 
COPY AuctionService/AuctionService.csproj AuctionService/AuctionService.csproj
COPY GatewayService/GatewayService.csproj GatewayService/GatewayService.csproj
COPY Contracts/Contracts.csproj Contracts/Contracts.csproj
COPY IdentityService/IdentityService.csproj IdentityService/IdentityService.csproj
COPY SearchService/SearchService.csproj SearchService/SearchService.csproj
COPY BidService/BidService.csproj BidService/BidService.csproj
COPY NotificationService/NotificationService.csproj NotificationService/NotificationService.csproj 

#COPY ["AuctionService/AuctionService.csproj", "AuctionService/"]

#RUN dotnet restore "AuctionService/AuctionService.csproj"
RUN dotnet restore AuctionsMicroServices.sln
#COPY . .
COPY AuctionService AuctionService
COPY Contracts Contracts
WORKDIR "/app/AuctionService"
#WORKDIR "/src/AuctionService"
RUN dotnet publish "AuctionService.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","AuctionService.dll"]

#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "AuctionService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "AuctionService.dll"]
