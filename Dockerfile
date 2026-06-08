# =========================
# Etapa 1 - Build
# =========================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia solução e projeto
COPY VitaOrbitApi.slnx .
COPY VitaOrbitApi/VitaOrbitApi.csproj VitaOrbitApi/

# Restaura dependências
RUN dotnet restore VitaOrbitApi/VitaOrbitApi.csproj

# Copia todo o restante do código
COPY . .

# Publica a aplicação
WORKDIR /src/VitaOrbitApi

RUN dotnet publish \
    -c Release \
    -o /app/publish \
    --no-restore

# =========================
# Etapa 2 - Runtime
# =========================

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copia arquivos publicados
COPY --from=build /app/publish .

# Variável de ambiente obrigatória
ENV ASPNETCORE_URLS=http://+:8080

# Porta da API
EXPOSE 8080

# Usuário não privilegiado
RUN useradd -m appuser

USER appuser

ENTRYPOINT ["dotnet", "VitaOrbitApi.dll"]
