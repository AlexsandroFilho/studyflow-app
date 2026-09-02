# Etapa de compilação: usa o SDK completo somente para restaurar e publicar a API.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["StudyFlow.Api.csproj", "./"]
RUN dotnet restore "StudyFlow.Api.csproj"

COPY . .
RUN dotnet publish "StudyFlow.Api.csproj" --configuration Release --output /app/publish --no-restore

# Etapa de execução: imagem menor, contendo apenas o runtime e os arquivos publicados.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StudyFlow.Api.dll"]
