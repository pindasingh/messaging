FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY Consumer.Push/Consumer.Push.csproj Consumer.Push/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore Consumer.Push/Consumer.Push.csproj

# Copy source and build
COPY Consumer.Push/ Consumer.Push/
COPY Shared/ Shared/
RUN dotnet publish Consumer.Push/Consumer.Push.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Consumer.Push.dll"]
