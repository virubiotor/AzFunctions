FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS publish

COPY --from=mcr.microsoft.com/dotnet/core/sdk:3.1 /usr/share/dotnet /usr/share/dotnet
RUN true
COPY "src/DurableECommerceWorkflow.sln" "src/DurableECommerceWorkflow.sln"
RUN true
COPY "src/Workflows/Workflows.csproj" "src/Workflows/Workflows.csproj"
RUN true
COPY "src/DurableECommerceWeb/DurableECommerceWeb.csproj" "src/DurableECommerceWeb/DurableECommerceWeb.csproj"
RUN true
COPY "src/DurableECommerceTests/DurableECommerceTests.csproj" "src/DurableECommerceTests/DurableECommerceTests.csproj"

RUN dotnet restore "src/DurableECommerceWorkflow.sln"
COPY . .
RUN cd /src/Workflows && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish Workflows.csproj --output /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet:3.0 AS base

ENV AzureWebJobsScriptRoot=/home/site/wwwroot
ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=publish ["/home/site/wwwroot", "/home/site/wwwroot"]