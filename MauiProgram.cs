using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MoodJournal.Services;
using MoodJournal.Services.Interfaces;

namespace MoodJournal;

/// <summary>
/// MAUI application entry point and dependency injection configuration.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Creates and configures the MAUI application.
    /// </summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Add Blazor WebView for hybrid app
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // --------
        // REGISTER SERVICES (Dependency Injection)
        // ---------
        
        // Database - Singleton (one instance throughout app lifetime)
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        
        // More services will be added as we build and add each feature:
        

        return builder.Build();
    }
}