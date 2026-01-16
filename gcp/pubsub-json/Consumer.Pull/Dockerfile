FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY Consumer.Pull/Consumer.Pull.csproj Consumer.Pull/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore Consumer.Pull/Consumer.Pull.csproj

# Copy source and build
COPY Consumer.Pull/ Consumer.Pull/
COPY Shared/ Shared/
RUN dotnet publish Consumer.Pull/Consumer.Pull.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Consumer.Pull.dll"]
