FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
LABEL maintainer="pankov.egor26032005@yandex.ru"

# Аргумент для конфигурации сборки (исправлена опечатка в слове "configuration")
ARG BUILD_CONFIGURATION=Release

# Установка рабочей директории
WORKDIR /GrifMVD

# Копирование файлов приложения
COPY . .

# Восстановление зависимостей и сборка приложения
RUN dotnet restore "GrifMVD.csproj"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final

# Установка рабочей директории
WORKDIR /app

# Открытие порта
EXPOSE 80

# Копирование собранного приложения из предыдущего этапа
COPY --from=build /app .

# Установка точки входа для запуска приложения
ENTRYPOINT ["dotnet", "GrifMVD.dll"]
