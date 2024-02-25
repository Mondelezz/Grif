FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
LABEL maintainer="pankov.egor26032005@yandex.ru"
ENV ASPNETCORE_ENVIRONMENT=Development
ARG BUILD_CONFIGURATION=Release

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "GrifMVD.dll"]
