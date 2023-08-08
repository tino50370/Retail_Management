# Use the official ASP.NET Core runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET Core SDK image as the build image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Retail_Management.csproj", "./"]
RUN dotnet restore "Retail_Management.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Retail_Management.csproj" -c Release -o /app/build

# Publish the application to the /app/publish directory
FROM build AS publish
RUN dotnet publish "Retail_Management.csproj" -c Release -o /app/publish

# Use the base image and copy the published output from the publish stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Retail_Management.dll"]
