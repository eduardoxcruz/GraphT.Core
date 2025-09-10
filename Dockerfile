FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["GraphT.WebAPI/GraphT.WebAPI.csproj", "GraphT.WebAPI/"]
COPY ["GraphT.Controllers/GraphT.Controllers.csproj", "GraphT.Controllers/"]
COPY ["GraphT.UseCases/GraphT.UseCases.csproj", "GraphT.UseCases/"]
COPY ["GraphT.Presenters/GraphT.Presenters.csproj", "GraphT.Presenters/"]
COPY ["GraphT.Model/GraphT.Model.csproj", "GraphT.Model/"]
COPY ["GraphT.EfCore/GraphT.EfCore.csproj", "GraphT.EfCore/"]
COPY ["GraphT.IoC/GraphT.IoC.csproj", "GraphT.IoC/"]
COPY ["SeedWork/SeedWork.csproj", "SeedWork/"]

RUN dotnet restore "GraphT.WebAPI/GraphT.WebAPI.csproj"

COPY . .

WORKDIR "/src/GraphT.WebAPI"
RUN dotnet build "GraphT.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GraphT.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "GraphT.WebAPI.dll"]