FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

ENV ProjectName=LoremFooBar.SarifBitbucketPipe

WORKDIR /source

COPY Directory.Build.props .
COPY src/$ProjectName/$ProjectName.csproj .

RUN dotnet restore

COPY src/$ProjectName/. ./

RUN dotnet publish -c Release -o /app


FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS runtime

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "/app/LoremFooBar.SarifBitbucketPipe.dll"]
