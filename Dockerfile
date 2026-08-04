# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar solo los .csproj para restaurar dependencias
COPY Dressly.Domain/Dressly.Domain.csproj Dressly.Domain/
COPY Dressly.Web/Dressly.Application.csproj Dressly.Web/
COPY Dressly.Infrastructure/Dressly.Infrastructure.csproj Dressly.Infrastructure/
COPY Dressly/Dressly.Web.csproj Dressly/
RUN dotnet restore Dressly/Dressly.Web.csproj

# Copiar todo el código fuente
COPY . .

# Publicar en modo Release
RUN dotnet publish Dressly/Dressly.Web.csproj -c Release -o /app/publish --no-restore

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Dressly.Web.dll"]
