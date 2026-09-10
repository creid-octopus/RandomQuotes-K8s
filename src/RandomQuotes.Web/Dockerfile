# --- Stage 1: base ---
# Lightweight ASP.NET runtime-only image used as the final container.
# NOTE: This is NOT the SDK - it cannot build or restore packages.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-azurelinux3.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# --- Stage 2: build ---
# Full SDK image. The Docker build context must be the 'src' directory (one level
# above RandomQuotes.Web/) so that the COPY paths below resolve correctly.
# Expected context layout:
#   src/
#     RandomQuotes.Web/
#       RandomQuotes.Web.csproj
#       ...
FROM mcr.microsoft.com/dotnet/sdk:10.0-azurelinux3.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "RandomQuotes.sln"
# Build the full solution so both the web project and test project are compiled
RUN dotnet build "RandomQuotes.sln" --no-restore -c Release

# --- Stage 3: test ---
# If the app built cleanly, we can run tests before publishing.
# This allows the image to act as the sole CI before deployment push
FROM build AS test
RUN dotnet test "RandomQuotes.Tests/RandomQuotes.Tests.csproj" --no-build -c Release

# --- Stage 4: publish ---
# Continues from the test stage, ensuring tests pass before an image is produced.
# Output is written to /app/publish inside this stage's filesystem.
FROM test AS publish
RUN dotnet publish "RandomQuotes.Web/RandomQuotes.Web.csproj" --no-build -c Release -o /app/publish /p:UseAppHost=false

# --- Stage 5: final ---
# Returns to the lightweight base image and copies only the publish output.
# Nothing from the SDK or build stages is included, keeping the image small.
FROM base AS final
WORKDIR /app
# Copy publish output from Stage 4 into the runtime image at /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RandomQuotes.Web.dll"]
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT="Production"
