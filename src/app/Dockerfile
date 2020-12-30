FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as build-env
WORKDIR /app
ARG VERSION

COPY ./ ./

ARG VERSION
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out /p:Version=$VERSION

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine 
ENV LANG=en_US.UTF-8
ENV ASPNETCORE_URLS=http://*:80
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "App.dll"]