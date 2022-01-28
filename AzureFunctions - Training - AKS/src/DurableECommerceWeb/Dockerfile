FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
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
RUN dotnet build --no-restore -c Release "src/DurableECommerceWorkflow.sln"

FROM build as publish
RUN dotnet publish ./src/DurableECommerceWeb --no-restore --no-build -c Release -o /publish

FROM base AS final  
COPY --from=publish /publish .
ENTRYPOINT ["dotnet", "DurableECommerceWeb.dll"]