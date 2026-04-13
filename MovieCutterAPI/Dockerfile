FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MovieCutterAPI/MovieCutterAPI.csproj", "MovieCutterAPI/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Persistance/Persistance.csproj", "Persistance/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["NuGet.Docker.config", "./NuGet.config"]
COPY ["packages/", "/nuget-local/"]

RUN dotnet restore "MovieCutterAPI/MovieCutterAPI.csproj"

COPY . .
RUN dotnet publish "MovieCutterAPI/MovieCutterAPI.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

RUN apt-get update \
    && apt-get install -y ffmpeg ca-certificates curl \
    && curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_linux -o /usr/local/bin/yt-dlp \
    && chmod a+rx /usr/local/bin/yt-dlp \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MovieCutterAPI.dll"]
