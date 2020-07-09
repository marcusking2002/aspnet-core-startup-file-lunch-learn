# Overview
This is a write-up to convey how ASP.NET Core starts up and is set up to run an application. It will start at Main and end at the execution of ConfigureServices. A lot happens in the beginning of an ASP.NET Core's startup. This was written targeting an explanation of .NET Core 3.1, but is mostly applicable to .NET Core 2.0 and up to 3.1. Later versions have not yet been released at the time this was written, therefore this might apply in the future, but is unknown today.

## How does an ASP.NET Core Web Application start?
An ASP.NET Core application's entry point is the Program.cs file. Main is invoked externally. Main calls CreateHostBuilder to bootstrap the application.

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

`Host.CreateDefaultBuilder` aggregates configuration from several places including environment variables, command line args, and appsettings.json to load configuration for the application. The launchsettings.json file assist in telling the framework which environment the application is being bootstrapped in so it knows which configuration settings to read. The framework will read one of two environment variables set on the machine: the `DOTNET_ENVIRONMENT` or the `ASPNETCORE_ENVIRONMENT` variable, where `ASPNETCORE_ENVRIONMENT` overrides the value of `DOTNET_ENVIRONMENT`. And while this environment name can be any desired string value, .NET Core recognizes 3 names by default: "Development", "Staging", and "Production".

The environment object that is subsequently built and passed into the Startup.cs file has 3 extension methods that can check for environment with a return type of `bool`: `IsDevelopment()`, `IsStaging()`, and `IsProduction()`. If you have an environment name of `local`, you can use the `IsEnvironment()` extension method and pass it a string for validation. This check is a case-insensitive string match.

## What is the Startup.cs file?

ASP.NET Core apps use a Startup class at startup time to do 2 things: configure services to be injected by the framework when instantiating objects and configure the request processing pipeline. From MSDN:

> Optionally includes a ConfigureServices method to configure the app's services. A service is a reusable component that provides app functionality. Services are registered in ConfigureServices and consumed across the app via dependency injection (DI) or ApplicationServices.
Includes a Configure method to create the app's request processing pipeline.
ConfigureServices and Configure are called by the ASP.NET Core runtime when the app starts


