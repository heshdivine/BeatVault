# STAGE 1: Build the App
# We now pull the SDK for .NET 10 (The correct "Kitchen" for your code)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code and build
COPY . ./
RUN dotnet publish -c Release -o out

# STAGE 2: Run the App
# We use the runtime for .NET 10 (The correct "Dining Table")
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/out .

# Expose the port (Standard is usually 8080)
EXPOSE 8080

# START COMMAND
# ⚠️ MAKE SURE "BeatVault.API.dll" MATCHES YOUR OUTPUT DLL NAME
ENTRYPOINT ["dotnet", "BeatVault.API.dll"]