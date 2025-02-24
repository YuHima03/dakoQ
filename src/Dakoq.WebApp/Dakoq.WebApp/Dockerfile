# デバッグ コンテナーをカスタマイズする方法と、Visual Studio がこの Dockerfile を使用してより高速なデバッグのためにイメージをビルドする方法については、https://aka.ms/customizecontainer をご覧ください。

# このステージは、VS から高速モードで実行するときに使用されます (デバッグ構成の既定値)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Build dotnet-knoq by @YuHima03
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dotnet_knoq
WORKDIR /libs
RUN git clone https://github.com/YuHima03/dotnet-knoq.git
WORKDIR /libs/dotnet-knoq
RUN dotnet build -o bin -f net9.0 -c Release

# Prepare for build.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS prepare
WORKDIR /src
COPY --from=dotnet_knoq /libs libs/
COPY ["src/Dakoq.WebApp/Dakoq.WebApp/Dakoq.WebApp.csproj", "src/Dakoq.WebApp/Dakoq.WebApp/"]
COPY ["src/Dakoq.WebApp/Dakoq.WebApp.Client/Dakoq.WebApp.Client.csproj", "src/Dakoq.WebApp/Dakoq.WebApp.Client/"]
RUN dotnet restore "./src/Dakoq.WebApp/Dakoq.WebApp/Dakoq.WebApp.csproj"
COPY . .

# Build css files with TailwindCSS-CLI.
FROM node:lts-alpine AS css_build
WORKDIR /src
COPY --from=prepare /src .
WORKDIR "/src/src/Dakoq.WebApp/Dakoq.WebApp"
RUN npm i
RUN npx @tailwindcss/cli -i "./Styles/app.css" -o "./wwwroot/css/app.css"

# Build the service project.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --from=css_build /src .
WORKDIR "/src/src/Dakoq.WebApp/Dakoq.WebApp"
RUN dotnet build "./Dakoq.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the service project to copy to the final stage.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dakoq.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Provide application binary.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dakoq.WebApp.dll"]
