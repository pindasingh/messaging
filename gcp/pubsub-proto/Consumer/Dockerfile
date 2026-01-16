FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy proto and project files
COPY Proto/ Proto/
COPY Consumer/Consumer.csproj Consumer/
RUN dotnet restore Consumer/Consumer.csproj

# Copy source and build
COPY Consumer/ Consumer/
RUN dotnet publish Consumer/Consumer.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Consumer.dll"]
