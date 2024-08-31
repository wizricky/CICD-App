# syntax=docker/dockerfile:1

# Build stage
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Copy the entire solution
COPY . /source
WORKDIR /source/FlexForge

# Restore NuGet packages and build the entire solution
ARG TARGETARCH
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore FlexForge.sln \
 && dotnet publish FlexForge.Web.csproj -c Release -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Install ICU packages to enable globalization support
RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

# Copy the build artifacts from the build stage
COPY --from=build /app .

# Set the user (assuming $APP_UID is set as an environment variable)
USER $APP_UID

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "FlexForge.Web.dll"]
