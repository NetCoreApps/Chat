FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
COPY src /app
WORKDIR /app

RUN dotnet restore --configfile NuGet.Config
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY --from=build-env /app/Chat/out .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "Chat.dll"]