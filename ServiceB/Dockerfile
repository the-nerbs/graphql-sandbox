ARG CONFIG=Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG CONFIG
WORKDIR /src

# Note: Common should probably come from a nuget feed
# instead of being build for each service separately.
COPY [ "Common/Common.csproj", "Common/Common.csproj" ]
COPY [ "ServiceB/ServiceB.csproj", "ServiceB/ServiceB.csproj" ]
COPY [ "*.props", "." ]
RUN dotnet restore "ServiceB/ServiceB.csproj"

COPY [ "Common/", "Common/." ]
COPY [ "ServiceB/", "ServiceB/." ]
RUN dotnet build "ServiceB/ServiceB.csproj" -c "$CONFIG" -o /app/build


FROM build as debug
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
WORKDIR /app/build
ENTRYPOINT [ "dotnet", "ServiceB.dll" ]


FROM build AS publish
RUN dotnet publish "ServiceB/ServiceB.csproj" -c "$CONFIG" -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "ServiceB.dll" ]
