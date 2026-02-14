FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["ToDo.API/ToDo.API.csproj", "ToDo.API/"]
COPY ["ToDo.Application/ToDo.Application.csproj", "ToDo.Application/"]
COPY ["ToDo.Domain/ToDo.Domain.csproj", "ToDo.Domain/"]
COPY ["ToDo.Infrastructure/ToDo.Infrastructure.csproj", "ToDo.Infrastructure/"]

RUN dotnet restore "./ToDo.API/ToDo.API.csproj"

COPY . .

WORKDIR "/src/ToDo.API"
RUN dotnet build "./ToDo.API.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/ToDo.API"
RUN dotnet publish "./ToDo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "ToDo.API.dll"]