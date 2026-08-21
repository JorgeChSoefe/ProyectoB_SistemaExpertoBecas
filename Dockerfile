FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ProyectoB.SistemaExpertoBecas.csproj"
RUN dotnet publish "ProyectoB.SistemaExpertoBecas.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProyectoB.SistemaExpertoBecas.dll"]
