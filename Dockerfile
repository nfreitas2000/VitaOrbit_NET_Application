# ==========================
# STAGE 1 - BUILD
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia solução e projeto
COPY VitaOrbit.slnx .
COPY VitaOrbitApi/VitaOrbitApi.csproj VitaOrbitApi/

# Restaura dependências
RUN dotnet restore VitaOrbitApi/VitaOrbitApi.csproj

# Copia todo o restante
COPY . .

# Publica aplicação
RUN dotnet publish VitaOrbitApi/VitaOrbitApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ==========================
# STAGE 2 - RUNTIME
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Diretório de trabalho (requisito)
WORKDIR /app

# Variável de ambiente (requisito)
ENV ASPNETCORE_URLS=http://+:8080

# Copia arquivos publicados
COPY --from=build /app/publish .

# Porta da aplicação (requisito)
EXPOSE 8080

# Usuário não privilegiado (requisito)
RUN addgroup --system vitaorbit && \
    adduser --system --ingroup vitaorbit appuser

RUN chown -R appuser:vitaorbit /app

USER appuser

ENTRYPOINT ["dotnet", "VitaOrbitApi.dll"]
