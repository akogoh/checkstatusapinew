FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ["checkstastusapinew.csproj", "./"]
RUN dotnet restore "./checkstastusapinew.csproj"

# copy everything else and build
COPY . .
RUN dotnet publish "checkstastusapinew.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS="http://+:80"
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 80
ENTRYPOINT ["dotnet", "checkstastusapinew.dll"]
