FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY NeuralStyle.Service/NeuralStyle.Service.csproj NeuralStyle.Service/
RUN dotnet restore NeuralStyle.Service/NeuralStyle.Service.csproj
COPY . .
WORKDIR /src/NeuralStyle.Service
RUN dotnet build NeuralStyle.Service.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish NeuralStyle.Service.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NeuralStyle.Service.dll"]
