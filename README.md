# infra-dataverse-api
Dataverse API Client

## Use ClientId and ClientSecret Authorization:

## Add NuGet packages
```
dotnet add package GarageGroup.Infra.Dataverse.Api
dotnet add package GarageGroup.Infra.Http.SocketsHandlerProvider
```

## Configure appsettings.json
```
{
  "Dataverse": {
    "ServiceUrl": "",
    "AuthTenantId": "",
    "AuthClientId": "",
    "AuthClientSecret": ""
  }
}
```

## Configure application:
```
public static Dependency<IDataverseApiClient> UseDataverseApiHandler()
    =>
    PrimaryHandler.UseStandardSocketsHttpHandler()
    .UseLogging("DataverseApi")
    .UseDataverseApiClient("Dataverse");
```
