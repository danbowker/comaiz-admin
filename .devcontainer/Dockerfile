# Use the official .NET 9.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install additional tools
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends \
        postgresql-client \
        curl \
        wget \
        git \
        lsof \
        pkill \
    && apt-get autoremove -y && apt-get clean -y && rm -rf /var/lib/apt/lists/*

# Install Entity Framework Core tools globally
RUN dotnet tool install --global dotnet-ef

# Add dotnet tools to PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Set the working directory
WORKDIR /workspace

# Configure git (for devcontainer)
RUN git config --global --add safe.directory /workspace