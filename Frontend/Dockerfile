FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ProductCrudBlazor.csproj", "./"]
RUN dotnet restore "ProductCrudBlazor.csproj"
COPY . .
RUN dotnet build "ProductCrudBlazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductCrudBlazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ProductCrudBlazor.dll"]