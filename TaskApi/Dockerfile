FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["TaskApi.csproj", "./"]
RUN dotnet restore "TaskApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TaskApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskApi.dll"]
