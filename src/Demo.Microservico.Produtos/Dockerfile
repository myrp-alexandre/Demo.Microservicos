#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# 01 ETAPA
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /build
EXPOSE 9030
EXPOSE 9031
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

# 02 ETAPA
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Demo.Microservico.Produtos.dll"]

#FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
#WORKDIR /src
#COPY ["Demo.Microservico.Produtos/Demo.Microservico.Produtos.csproj", "Demo.Microservico.Produtos/"]
#RUN dotnet restore "Demo.Microservico.Produtos/Demo.Microservico.Produtos.csproj"
#COPY . .
#WORKDIR "/src/Demo.Microservico.Produtos"
#RUN dotnet build "Demo.Microservico.Produtos.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "Demo.Microservico.Produtos.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Demo.Microservico.Produtos.dll"]