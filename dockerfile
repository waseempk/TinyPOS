# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TinyPOSApp.csproj", "./"]
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app .
ENTRYPOINT ["dotnet", "TinyPOSApp.dll"]