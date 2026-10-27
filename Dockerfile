FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY lender_club.csproj ./

RUN dotnet restore lender_club.csproj

COPY . .

RUN dotnet publish lender_club.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "lender_club.dll"]