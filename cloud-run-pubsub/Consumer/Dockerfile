FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY Consumer/Consumer.csproj Consumer/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore Consumer/Consumer.csproj

# Copy source and build
COPY Consumer/ Consumer/
COPY Shared/ Shared/
RUN dotnet publish Consumer/Consumer.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Run as non-root user
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Consumer.dll"]
