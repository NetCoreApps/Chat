FROM microsoft/dotnet:3.0-sdk AS build-env
COPY src /app
WORKDIR /app

RUN dotnet restore --configfile NuGet.Config
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:3.0-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/Chat/out .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "Chat.dll"]