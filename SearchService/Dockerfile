#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["SearchService/SearchService.csproj", "SearchService/"]
#COPY ["Contracts/Contracts.csproj", "Contracts/"]
#RUN dotnet restore "SearchService/SearchService.csproj"
#COPY . .
#WORKDIR "/src/SearchService"
#RUN dotnet build "SearchService.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "SearchService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "SearchService.dll"]
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
EXPOSE 80
COPY AuctionsMicroServices.sln AuctionsMicroServices.sln 
COPY AuctionService/AuctionService.csproj AuctionService/AuctionService.csproj
COPY GatewayService/GatewayService.csproj GatewayService/GatewayService.csproj
COPY Contracts/Contracts.csproj Contracts/Contracts.csproj
COPY IdentityService/IdentityService.csproj IdentityService/IdentityService.csproj
COPY SearchService/SearchService.csproj SearchService/SearchService.csproj

RUN dotnet restore AuctionsMicroServices.sln

COPY SearchService SearchService
COPY Contracts Contracts

WORKDIR "/app/SearchService"
#WORKDIR "/src/AuctionService"
RUN dotnet publish "SearchService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","SearchService.dll"]





#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#WORKDIR /app
#COPY --from=build /app/publish .
#ENTRYPOINT ["dotnet","AuctionService.dll"]