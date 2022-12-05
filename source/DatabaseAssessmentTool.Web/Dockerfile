#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

ENV NODE_VERSION=16.18.0
RUN apt install -y curl
RUN curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
ENV NVM_DIR=/root/.nvm
RUN . "$NVM_DIR/nvm.sh" && nvm install ${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm use v${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm alias default v${NODE_VERSION}
ENV PATH="/root/.nvm/versions/node/v${NODE_VERSION}/bin/:${PATH}"
RUN node --version
RUN npm --version

WORKDIR /src
COPY ["DatabaseAssessmentTool.Web/package.json", "DatabaseAssessmentTool.Web/"]
COPY ["DatabaseAssessmentTool.Web/package-lock.json", "DatabaseAssessmentTool.Web/"]
WORKDIR /src/DatabaseAssessmentTool.Web
RUN npm ci
WORKDIR /src

COPY ["DatabaseAssessmentTool.Web/DatabaseAssessmentTool.Web.csproj", "DatabaseAssessmentTool.Web/"]
RUN dotnet restore "DatabaseAssessmentTool.Web/DatabaseAssessmentTool.Web.csproj"
COPY . .
WORKDIR "/src/DatabaseAssessmentTool.Web"
RUN dotnet build "DatabaseAssessmentTool.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseAssessmentTool.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseAssessmentTool.Web.dll"]