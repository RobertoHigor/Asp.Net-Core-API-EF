FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 50001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/CoreCodeCamp.csproj", "src/"]
RUN dotnet restore "src/CoreCodeCamp.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "CoreCodeCamp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreCodeCamp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreCodeCamp.dll"]
