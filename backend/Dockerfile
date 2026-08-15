FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for caching layers
COPY ["BRC.AssignmentManagement.sln", "./"]
COPY ["src/BRC.Domain/BRC.Domain.csproj", "src/BRC.Domain/"]
COPY ["src/BRC.Application/BRC.Application.csproj", "src/BRC.Application/"]
COPY ["src/BRC.Infrastructure/BRC.Infrastructure.csproj", "src/BRC.Infrastructure/"]
COPY ["src/BRC.API/BRC.API.csproj", "src/BRC.API/"]
RUN dotnet restore src/BRC.API/BRC.API.csproj

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/src/BRC.API"
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 (standard for Render and cloud providers)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BRC.API.dll"]
