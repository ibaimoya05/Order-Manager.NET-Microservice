FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080   

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["order_manager.csproj", "./"]
RUN dotnet restore 
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Ejecuta la aplicación en el puerto 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "order_manager.dll"]