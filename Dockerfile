FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as build

ENV ProjectName=LoremFooBar.SarifBitbucketPipe

WORKDIR /source

COPY Directory.Build.props .
COPY src/$ProjectName/$ProjectName.csproj .

RUN dotnet restore

COPY src/$ProjectName/. ./

RUN dotnet publish -c Release -o /app


FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine as runtime

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "/app/LoremFooBar.SarifBitbucketPipe.dll"]
