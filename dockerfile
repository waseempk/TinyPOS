# Build stage
FROM ://microsoft.com AS build
WORKDIR /src
COPY ["TinyPOSApp.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

# Final stage
FROM ://microsoft.com
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "TinyPOSApp.dll"]
