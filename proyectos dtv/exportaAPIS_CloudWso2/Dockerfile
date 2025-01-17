#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["exportaAPIS_CloudWso2/exportaAPIS_CloudWso2.csproj", "exportaAPIS_CloudWso2/"]
RUN dotnet restore "exportaAPIS_CloudWso2/exportaAPIS_CloudWso2.csproj"
COPY . .
WORKDIR "/src/exportaAPIS_CloudWso2"
RUN dotnet build "exportaAPIS_CloudWso2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "exportaAPIS_CloudWso2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "exportaAPIS_CloudWso2.dll"]