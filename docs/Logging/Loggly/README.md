[![EMG.Extensions.Logging.Loggly](https://img.shields.io/nuget/v/EMG.Extensions.Logging.Loggly.svg)](https://www.nuget.org/packages/EMG.Extensions.Logging.Loggly)

# EMG Logging Extension for Loggly
This package contains an implementation of Microsoft's `ILogger` stack to support Loggly.
To use this package, you just need to register the logger provider to the `ILoggingBuilder`.

## Registering the provider
```csharp
public void Everything_in_code()
{
    var services = new ServiceCollection();

    services.AddLogging(logging => 
    {
        logging.AddLoggly(() => new LogglyOptions {
            ApiKey = "Loggly API key",
            ApplicationName = "My Application Name"
        });
    });
}

public void From_configuration()
{
    IConfigurationRoot configuration = ...
    var services = new ServiceCollection();

    services.AddLogging(logging => 
    {
        logging.AddLoggly(configuration.GetSection("Loggly"));
    });
}

public void From_configuration_and_customized()
{
    IConfigurationRoot configuration = ...
    var services = new ServiceCollection();

    services.AddLogging(logging => 
    {
        logging.AddLoggly(configuration.GetSection("Loggly"), options => 
        {
            // do something with the options you received
        });
    });
}
```

The samples using the configuration assume that a configuration equivalent to the following JSON document is registered to the application

```json
{
    "Loggly": {
        "ApiKey" : "Loggly API key",
        "ApplicationName" : "My Application Name"
    }
}
```

## Optional configuration
The following optional properties can be customized via configuration

|Property|Type|Default value|
|-|-|-|
|Environment|string|"Development"|
|Tags|string\[\]|empty array|
|SuppressExceptions|bool|false|
|LogglyHost|string|"logs-01.loggly.com"|
|LogglyScheme|string|"https"|

The following optional properties can only be customized via code

|Property|Type|Default value|
|-|-|-|
|ContentEncoding|Encoding|UTF8|

The following delegates can be customized via code

|Name|Parameters|Return type|
|-|-|-|
|Filter|CategoryName, EventId, LogLevel|bool|
|PreProcessMessage|LogglyMessage|-|