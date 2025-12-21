# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ITAssetManager.Backend.csproj", "./"]
RUN dotnet restore "ITAssetManager.Backend.csproj"
COPY . .
RUN dotnet build "ITAssetManager.Backend.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "ITAssetManager.Backend.csproj" -c Release -o /app/publish

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ITAssetManager.Backend.dll"]
