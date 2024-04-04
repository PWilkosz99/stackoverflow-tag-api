#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StackoverflowTagApi/StackoverflowTagApi.csproj", "StackoverflowTagApi/"]
RUN dotnet restore "./StackoverflowTagApi/./StackoverflowTagApi.csproj"
COPY . .
WORKDIR "/src/StackoverflowTagApi"
RUN dotnet build "./StackoverflowTagApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef
RUN dotnet ef database update
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StackoverflowTagApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StackoverflowTagApi.dll"]