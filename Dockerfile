# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["Domain/Domain.csproj",                                                                                      "Domain/"]
COPY ["Application/Application.csproj",                                                                            "Application/"]
COPY ["Infrastructure/Infrastructure.csproj",                                                                      "Infrastructure/"]
COPY ["Backend .NET Developer - Technical Assessment Task/Backend .NET Developer - Technical Assessment Task.csproj", \
      "Backend .NET Developer - Technical Assessment Task/"]

RUN dotnet restore "Backend .NET Developer - Technical Assessment Task/Backend .NET Developer - Technical Assessment Task.csproj"

# Copy everything and publish
COPY . .
WORKDIR "/src/Backend .NET Developer - Technical Assessment Task"
RUN dotnet publish "Backend .NET Developer - Technical Assessment Task.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Backend .NET Developer - Technical Assessment Task.dll"]
