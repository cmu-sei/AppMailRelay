FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as dev

ENV ASPNETCORE_URLS=http://*:5000 \
    ASPNETCORE_ENVIRONMENT=DEVELOPMENT

COPY . /app
WORKDIR /app/src/AppMailRelay
RUN dotnet publish -o /app/dist
CMD [ "dotnet", "run" ]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as prod
COPY --from=dev /app/dist /app
COPY --from=dev /app/License.md /app
WORKDIR /app
EXPOSE 80/tcp
ENV ASPNETCORE_URLS=http://*:80
ENTRYPOINT [ "dotnet", "AppMailRelay.dll" ]
