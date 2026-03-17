# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY eCashMeUp/*.csproj ./eCashMeUp/
RUN dotnet restore ./eCashMeUp/eCashMeUp.csproj

# Copy everything else
COPY . ./
RUN dotnet publish ./eCashMeUp/eCashMeUp.csproj -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out ./

# Bind to Render's $PORT
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

ENTRYPOINT ["dotnet", "eCashMeUp.dll"]