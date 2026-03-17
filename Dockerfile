# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj first and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out ./

# Bind to PORT for Render
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

# Run the application
ENTRYPOINT ["dotnet", "eCashMeUp.dll"]