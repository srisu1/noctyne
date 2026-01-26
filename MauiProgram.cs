using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MoodJournal.Services;
using MoodJournal.Services.Interfaces;

namespace MoodJournal;

public static class MauiProgram
{
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

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Register services
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddScoped<IAvatarService, AvatarService>();
        builder.Services.AddScoped<ISecurityService, SecurityService>();
        builder.Services.AddScoped<IJournalService, JournalService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IMoodStickerService, MoodStickerService>();

        return builder.Build();
    }
}