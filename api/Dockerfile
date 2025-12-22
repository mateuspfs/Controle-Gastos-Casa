# Dockerfile para a API .NET
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos .csproj e restaura as dependências
COPY ["ControleGastosCasa.Api/ControleGastosCasa.Api.csproj", "ControleGastosCasa.Api/"]
COPY ["ControleGastosCasa.Application/ControleGastosCasa.Application.csproj", "ControleGastosCasa.Application/"]
COPY ["ControleGastosCasa.Domain/ControleGastosCasa.Domain.csproj", "ControleGastosCasa.Domain/"]
COPY ["ControleGastosCasa.Infrastructure/ControleGastosCasa.Infrastructure.csproj", "ControleGastosCasa.Infrastructure/"]

RUN dotnet restore "ControleGastosCasa.Api/ControleGastosCasa.Api.csproj"

# Copia todo o código e compila
COPY . .
WORKDIR "/src/ControleGastosCasa.Api"
RUN dotnet build "ControleGastosCasa.Api.csproj" -c Release -o /app/build

# Publica a aplicação
FROM build AS publish
RUN dotnet publish "ControleGastosCasa.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ControleGastosCasa.Api.dll"]

