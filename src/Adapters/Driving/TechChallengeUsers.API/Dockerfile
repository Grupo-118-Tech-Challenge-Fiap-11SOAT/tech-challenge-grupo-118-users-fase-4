# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Adapters/Driving/TechChallengeUsers.API/TechChallengeUsers.API.csproj", "Adapters/Driving/TechChallengeUsers.API/"]

COPY ["src/Core/Application/Application/Application.csproj", "Core/Application/Application/"]

COPY ["src/Core/Domain/Domain/Domain.csproj", "Core/Domain/Domain/"]

COPY ["src/Adapters/Driven/Infra.Database.SqlServer/Infra.Database.SqlServer.csproj", "Adapters/Driven/Infra.Database.SqlServer/"]

COPY ["src/Adapters/Driven/Infra.Password/Infra.Password.csproj", "src/Adapters/Driven/Infra.Password/"]

RUN dotnet restore "./Adapters/Driving/TechChallengeUsers.API/TechChallengeUsers.API.csproj"
COPY . .
WORKDIR "/src/src/Adapters/Driving/TechChallengeUsers.API"
RUN dotnet build "./TechChallengeUsers.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechChallengeUsers.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechChallengeUsers.API.dll"]