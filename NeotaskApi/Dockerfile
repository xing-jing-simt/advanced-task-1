FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
ENV RABBITMQ_HOST localhost
ENV RABBITMQ_PORT 31672
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["NeotaskApi.csproj", "./"]
RUN dotnet restore "NeotaskApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "NeotaskApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NeotaskApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NeotaskApi.dll"]
