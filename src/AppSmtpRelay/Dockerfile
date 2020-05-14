FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as dev

COPY . /app
WORKDIR /app/src/AppSmtpRelay
RUN dotnet publish -o /app/dist
CMD [ "dotnet", "run" ]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as prod
COPY --from=dev /app/dist /app
WORKDIR /app
EXPOSE 25/tcp
ENTRYPOINT [ "dotnet", "AppSmtpRelay.dll" ]
