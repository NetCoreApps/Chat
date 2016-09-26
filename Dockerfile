FROM microsoft/dotnet:latest
COPY src/Chat /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
ENV VIRTUAL_HOST chat-demo.layoric.org
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "run", "--server.urls", "http://0.0.0.0:5000"]