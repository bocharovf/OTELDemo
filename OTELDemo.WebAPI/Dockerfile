FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore OTELDemo.sln
# Build and publish a release
RUN dotnet publish OTELDemo.WebAPI/OTELDemo.WebAPI.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out/ .
ENTRYPOINT ["dotnet", "OTELDemo.WebAPI.dll"]