# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["tp3.csproj", "./"]
RUN dotnet restore "tp3.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "tp3.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "tp3.csproj" -c Release -o /app/publish

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tp3.dll"]
