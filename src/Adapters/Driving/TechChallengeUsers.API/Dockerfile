# Estágio de Execução (Runtime) - Já utiliza Alpine para leveza
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Estágio de Compilação (Build)
# Usamos a imagem cheia do SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["src/Adapters/Driving/TechChallengeUsers.API/TechChallengeUsers.API.csproj", "Adapters/Driving/TechChallengeUsers.API/"]
COPY ["src/Core/Application/Application/Application.csproj", "Core/Application/Application/"]
COPY ["src/Core/Domain/Domain/Domain.csproj", "Core/Domain/Domain/"]
COPY ["src/Adapters/Driven/Infra.Database.SqlServer/Infra.Database.SqlServer.csproj", "Adapters/Driven/Infra.Database.SqlServer/"]
COPY ["src/Adapters/Driven/Infra.Password/Infra.Password.csproj", "Adapters/Driven/Infra.Password/"]


RUN dotnet restore "Adapters/Driving/TechChallengeUsers.API/TechChallengeUsers.API.csproj"


COPY . .
WORKDIR "/src/src/Adapters/Driving/TechChallengeUsers.API"
RUN dotnet build "TechChallengeUsers.API.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "TechChallengeUsers.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechChallengeUsers.API.dll"]