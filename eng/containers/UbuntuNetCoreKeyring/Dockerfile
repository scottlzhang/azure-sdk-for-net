FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build

ENV \
    NO_AT_BRIDGE=1 \
    DOCKER_CONTAINER_NAME="ubuntu_netcore_keyring" \
    PSModuleAnalysisCachePath=/var/cache/microsoft/powershell/PSModuleAnalysisCache/ModuleAnalysisCache \
    POWERSHELL_DISTRIBUTION_CHANNEL=PSDocker-DotnetCoreSDK-Ubuntu-18.04

# Install GNOME keyring
RUN apt-get update \
    && apt-get install -y \
        libsecret-1-dev \
        dbus-x11 \
        gnome-keyring

# Install PowerShell && .net core runtime 2.1
RUN wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y powershell \
    && pwsh --version \
    && apt-get install -y dotnet-runtime-2.1 \
    && dotnet --list-runtimes