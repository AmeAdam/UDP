FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GrpcClient/GrpcClient.csproj", "GrpcClient/"]
RUN dotnet restore "GrpcClient/GrpcClient.csproj"
COPY . .
WORKDIR "/src/GrpcClient"
RUN dotnet build "GrpcClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GrpcClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

RUN apt update; apt install curl -y; \
curl https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb --output packages-microsoft-prod.deb; \
dpkg -i packages-microsoft-prod.deb; \
rm packages-microsoft-prod.deb
RUN apt update; apt install libmsquic -y; apt clean -y;

WORKDIR /app
COPY --from=build /src/GrpcClient/AmeCA.pem /etc/ssl/certs/AmeCA.pem

#COPY --from=build /src/GrpcClient/AmeCA.crt /usr/local/share/ca-certificates/AmeCA.crt
#RUN echo AmeCA.crt >> /etc/ca-certificates.conf
#RUN update-ca-certificates

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrpcClient.dll"]
