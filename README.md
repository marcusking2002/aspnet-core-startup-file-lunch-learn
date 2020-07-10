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

`Host.CreateDefaultBuilder` aggregates configuration from several places including environment variables, command line args, and appsettings.json to load configuration for the application. The launchsettings.json file assist in telling the framework which environment the application is being bootstrapped in so it knows which configuration settings to read in a local environment. The framework will read one of two environment variables set on the machine: the `DOTNET_ENVIRONMENT` or the `ASPNETCORE_ENVIRONMENT` variable, where `ASPNETCORE_ENVRIONMENT` overrides the value of `DOTNET_ENVIRONMENT`. And while this environment name can be any desired string value, .NET Core recognizes 3 names by default: "Development", "Staging", and "Production".

The environment object that is subsequently built and passed into the Startup.cs file has 3 extension methods that can check for environment with a return type of `bool`: `IsDevelopment()`, `IsStaging()`, and `IsProduction()`. If you have an environment name of `local`, you can use the `IsEnvironment()` extension method and pass it a string for validation. This check is a case-insensitive string match.

## What is the Startup.cs file?

ASP.NET Core apps use a Startup class at startup time to do 2 things: configure services to be injected by the framework when instantiating objects and configure the request processing pipeline with middleware. From MSDN:

> Optionally includes a ConfigureServices method to configure the app's services. A service is a reusable component that provides app functionality. Services are registered in ConfigureServices and consumed across the app via dependency injection (DI) or ApplicationServices.
Includes a Configure method to create the app's request processing pipeline.
ConfigureServices and Configure are called by the ASP.NET Core runtime when the app starts

### What is middleware?

What is the processing pipeline? A series of middleware components that can act on Http request. There are 2 ways to write middleware. The first is inline and the second is as reusable classes. Inline middleware is shown below for brevity:
```csharp
// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsEnvironment("DEVELOPment"))
    {
        app.UseStaticFiles();
    }

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.Use(async (context, next) =>
    {
        context.Response.Cookies.Append("SOME_COOKIE", DateTime.Now.Ticks.ToString());

        await context.Response.WriteAsync("You have total control over the request pipeline inline.");

        await next.Invoke();
    });

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

If you ran this code you'd see SOME_COOKIE have the tick value of the current time. If you look real closely with Dev Tools you'll see the browser flash 2 numbers on one page reload. Why is that? Well, the browser makes two requests on a page refresh: one for the HTML document and one for the fav icon. Just a reminder to be very aware that EVERY request will go through your request pipeline you set up in the order it is setup.

If we change the order of this middleware, and put the middleware below `UseEndpoints` our hijacking of the response body disappears. The interesting part is if you were to put a breakpoint in place, your inline middleware would still cause a break while debugging. Since `MapControllers()` starts writing to the Response body before you, you have interceded too late and your message will not be read.

We are using the `.Use()` extension method above, but you can also use `.Run` to achieve the same thing, with one small caveat. Middleware components that break the chain of execution are called terminal middleware, and `.Run` middleware is as such. It does not get passed a `next` to invoke. if you forget to call `next.Invoke()` on a piece of middleware, you are in essence terminating the pipeline. The `.Use` middleware is a convention in ASP.NET Core to convey execution of the pipeline will continue, whereas `.Run` conveys the execution will terminate.

One more thing to note on order, there is an execution order that is appropriate and recommended, especially when dealing with security. Please refer below to the .NET Core Fundamentals for more detail.

There is a feature-rich set of middleware that is already available from the framework as well as open-source software filling in the gaps.

### What does ConfigureServices do?
`ConfigureServices()` allows you to set up your IoC container. It also allows you set up connections to data repositories, get configuration values that were retrieved from execution of the Program.cs file, and much more. This also uses the same extension method pattern seen in the Configure method for the `IApplicationBuilder` class. The IoC container is based off Microsoft's old IoC container, Unity. While syntatically similar to Unity, there have been vast performance enhancements.

Things to note, the built-in IoC container lets you specify lifetime scope of components: Transient, Scoped, and Singleton. Here is a good example of how this works:
```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddSingleton<IConnectionManager, ConnectionManager>(); //instantiation per app lifetime
    services.AddScoped<IDataRepo, DataRepo>(); // instantiation per web request 
    //services.AddTransient => instantiation every time one is needed. 
}
```

# References
.NET Core Fundamentals: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-3.1&tabs=windows

Dependency Injection: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1

Multiple Environments: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.1

