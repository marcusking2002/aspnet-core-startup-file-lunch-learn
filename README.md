## How does an ASP.NET Core Web Application start?
An ASP.NET Core application's entry point is the Program.cs file. Main is invoked externally. Main CreateHostBuilder to bootstrap the application.

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

CreateDefaultBuilder aggregates configuration from several places including environment variables, command line args, and appsettings.json to load configuration for the application. 

## What is the Startup.cs file?

ASP.NET Core apps use a Startup class at startup time to do 2 things: configure services to be injected by the framework when instantiating objects and configure the request processing pipeline. From MSDN:

> Optionally includes a ConfigureServices method to configure the app's services. A service is a reusable component that provides app functionality. Services are registered in ConfigureServices and consumed across the app via dependency injection (DI) or ApplicationServices.
Includes a Configure method to create the app's request processing pipeline.
ConfigureServices and Configure are called by the ASP.NET Core runtime when the app starts


