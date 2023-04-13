FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 18080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GrpcService/GrpcService.csproj", "GrpcService/"]
RUN dotnet restore "GrpcService/GrpcService.csproj"
COPY . .
WORKDIR "/src/GrpcService"
RUN dotnet build "GrpcService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GrpcService.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM base AS final

RUN apt update; apt install curl -y; \
curl https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb --output packages-microsoft-prod.deb; \
dpkg -i packages-microsoft-prod.deb; \
rm packages-microsoft-prod.deb
RUN apt update; apt install libmsquic -y; apt clean -y;

WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/GrpcService/ame.pfx /certs/
ENTRYPOINT ["dotnet", "GrpcService.dll"]

#docker build -t test -f .\GrpcService\Dockerfile .
#docker run --network ame -p 18080:18080 -p 18081:18081 -p 18082:18082 --name ame test

