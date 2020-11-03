FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["SpotCC.csproj", ""]
RUN dotnet restore "SpotCC.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "SpotCC.csproj" -c Release -o /app

RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq

FROM build AS publish
RUN dotnet publish "SpotCC.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SpotCC.dll"]