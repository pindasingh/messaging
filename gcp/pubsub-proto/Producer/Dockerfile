FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy proto and project files
COPY Proto/ Proto/
COPY Producer/Producer.csproj Producer/
RUN dotnet restore Producer/Producer.csproj

# Copy source and build
COPY Producer/ Producer/
RUN dotnet publish Producer/Producer.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Producer.dll"]
