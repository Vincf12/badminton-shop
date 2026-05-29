FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MyAPI.csproj ./
RUN dotnet restore MyAPI.csproj

COPY . .
RUN dotnet publish MyAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5211
EXPOSE 5211

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyAPI.dll"]