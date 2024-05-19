FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DockerSample.csproj", "./"]
RUN dotnet restore "DockerSample.csproj"
COPY . .

WORKDIR "/src"
RUN dotnet build "DockerSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerSample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ConnectionString="Server=db;User ID=root;Password=root;Database=dockersample;"
ENTRYPOINT ["dotnet", "DockerSample.dll"]