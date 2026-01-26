using MoodJournal.Services.Interfaces;

namespace MoodJournal;

public partial class App : Application
{
    public App(IDatabaseService databaseService)
    {
        InitializeComponent();
        
        MainPage = new MainPage();
        
        // Initialize database synchronously (blocking but necessary)
        Task.Run(async () => 
        {
            try 
            {
                await databaseService.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB Error: {ex.Message}");
            }
        }).GetAwaiter().GetResult();
    }
}
