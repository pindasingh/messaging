FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY Producer/Producer.csproj Producer/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore Producer/Producer.csproj

# Copy source and build
COPY Producer/ Producer/
COPY Shared/ Shared/
RUN dotnet publish Producer/Producer.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Run as non-root user
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Producer.dll"]
