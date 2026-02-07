FROM mcr.microsoft.com/devcontainers/base:ubuntu

ARG DOTNET_VERSION=9.0

RUN apt-get update && apt-get install -y \
    curl \
    ca-certificates \
    git \
    fish \
    && rm -rf /var/lib/apt/lists/*

RUN curl -fsSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh \
    && chmod +x dotnet-install.sh \
    && ./dotnet-install.sh \
        --channel ${DOTNET_VERSION} \
        --install-dir /usr/share/dotnet \
    && rm dotnet-install.sh

ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH=$PATH:/usr/share/dotnet
