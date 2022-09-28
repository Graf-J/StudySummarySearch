# [Stage 1] Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /source
COPY . .
RUN dotnet restore "./StudySummarySearch.API/StudySummarySearch.API.csproj"
RUN dotnet publish "./StudySummarySearch.API/StudySummarySearch.API.csproj" -c Release -o /app

# [Stage 2] Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app ./
ENTRYPOINT ["dotnet", "StudySummarySearch.API.dll"]