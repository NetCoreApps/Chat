FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Chat/*.csproj ./Chat/
RUN dotnet restore

# copy everything else and build app
COPY Chat/. ./Chat/
WORKDIR /app/Chat
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/Chat/out ./
ENTRYPOINT ["dotnet", "Chat.dll"]