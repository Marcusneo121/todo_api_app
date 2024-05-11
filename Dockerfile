FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

#Copy csproj and restore as distinct layers
COPY *.csproj ./

RUN dotnet restore

#COPY everything else and build
COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build-env /app/out ./

ENTRYPOINT [ "dotnet", "todo_api_app.dll" ]