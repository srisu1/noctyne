using SQLite;
using MoodJournal.Models;

namespace MoodJournal.Services;

/// <summary>
/// Manages all SQLite database operations for MoodJournal
/// Enhanced with detailed logging for debugging
/// </summary>
public class DatabaseService : Services.Interfaces.IDatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

  
    //Constructor that sets up database file path
    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "moodjournal.db3");
        Console.WriteLine($"📁 Database path set to: {_dbPath}");
    }

 
    //Initialize database connection and create all tables that is called once at app startup
    public async Task InitializeAsync()
    {
        Console.WriteLine(" InitializeAsync called");
        
        // Prevent multiple initializations
        if (_database != null)
        {
            Console.WriteLine(" Database already initialized, skipping");
            return;
        }

        try
        {
            Console.WriteLine(" Creating database connection...");
            
            // Create database connection
            _database = new SQLiteAsyncConnection(_dbPath);
            Console.WriteLine("Database connection created");

            // Create all tables (SQLite auto creates if not exists)
            Console.WriteLine("Creating tables...");
            
            Console.WriteLine("Creating AppSettings table...");
            await _database.CreateTableAsync<AppSetting>();
            Console.WriteLine(" AppSettings table created");
            
            Console.WriteLine("Creating UserProfile table...");
            await _database.CreateTableAsync<UserProfile>();
            Console.WriteLine("UserProfile table created");
            
            Console.WriteLine("Creating AvatarConfiguration table...");
            await _database.CreateTableAsync<AvatarConfiguration>();
            Console.WriteLine("AvatarConfiguration table created");
            
            Console.WriteLine("Creating SecurityCredential table...");
            await _database.CreateTableAsync<SecurityCredential>();
            Console.WriteLine("SecurityCredential table created");

            Console.WriteLine("Seeding default settings...");
            await SeedDefaultSettingsAsync();
            Console.WriteLine("Default settings seeded");
            
            Console.WriteLine("Database initialization complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database initialization error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }


    // Get database connection (ensures initialization first)
    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database == null)
        {
            Console.WriteLine("Database not initialized, initializing now...");
            await InitializeAsync();
        }

        return _database!;
    }

    #region Default Settings


    //Seed default application settings on first launch
    private async Task SeedDefaultSettingsAsync()
    {
        try
        {
            var existingSettings = await _database!.Table<AppSetting>().CountAsync();
            Console.WriteLine($"  - Found {existingSettings} existing settings");
            
            if (existingSettings > 0)
            {
                Console.WriteLine("  - Settings already exist, skipping seed");
                return;
            }

            Console.WriteLine("  - Inserting default settings...");
            var defaultSettings = new List<AppSetting>
            {
                new() { SettingKey = SettingKeys.Theme, SettingValue = "light" },
                new() { SettingKey = SettingKeys.IsFirstLaunch, SettingValue = "true" },
                new() { SettingKey = SettingKeys.OnboardingCompleted, SettingValue = "false" },
                new() { SettingKey = SettingKeys.AvatarCompleted, SettingValue = "false" },
                new() { SettingKey = SettingKeys.SecurityCompleted, SettingValue = "false" },
                new() { SettingKey = SettingKeys.SecurityEnabled, SettingValue = "false" },
                new() { SettingKey = SettingKeys.FontSize, SettingValue = "medium" },
                new() { SettingKey = SettingKeys.AccentColor, SettingValue = "#F490AF" }
            };

            await _database.InsertAllAsync(defaultSettings);
            Console.WriteLine($"Inserted {defaultSettings.Count} default settings");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding settings: {ex.Message}");
            throw;
        }
    }

    #endregion

    #region Settings Methods

    public async Task<string?> GetSettingAsync(string key)
    {
        try
        {
            var db = await GetConnectionAsync();
            var setting = await db.Table<AppSetting>()
                .Where(s => s.SettingKey == key)
                .FirstOrDefaultAsync();

            return setting?.SettingValue;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting setting '{key}': {ex.Message}");
            return null;
        }
    }

    public async Task SetSettingAsync(string key, string value)
    {
        try
        {
            var db = await GetConnectionAsync();
            var setting = await db.Table<AppSetting>()
                .Where(s => s.SettingKey == key)
                .FirstOrDefaultAsync();

            if (setting != null)
            {
                setting.SettingValue = value;
                setting.UpdatedAt = DateTime.UtcNow;
                await db.UpdateAsync(setting);
                Console.WriteLine($"Updated setting '{key}' = '{value}'");
            }
            else
            {
                await db.InsertAsync(new AppSetting
                {
                    SettingKey = key,
                    SettingValue = value
                });
                Console.WriteLine($"Created setting '{key}' = '{value}'");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting '{key}': {ex.Message}");
            throw;
        }
    }

    public async Task<bool> GetBoolSettingAsync(string key, bool defaultValue = false)
    {
        var value = await GetSettingAsync(key);
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        return bool.TryParse(value, out var result) && result;
    }

    public async Task SetBoolSettingAsync(string key, bool value)
    {
        await SetSettingAsync(key, value.ToString().ToLower());
    }

    #endregion

    #region User Profile Methods

    public async Task<UserProfile?> GetUserProfileAsync()
    {
        try
        {
            var db = await GetConnectionAsync();
            var profile = await db.Table<UserProfile>().FirstOrDefaultAsync();
            
            if (profile != null)
            {
                Console.WriteLine($"Found user profile: {profile.Name} (ID: {profile.Id})");
            }
            else
            {
                Console.WriteLine("No user profile found");
            }
            
            return profile;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user profile: {ex.Message}");
            return null;
        }
    }

    public async Task<UserProfile> SaveUserProfileAsync(UserProfile profile)
    {
        try
        {
            var db = await GetConnectionAsync();
            profile.UpdatedAt = DateTime.UtcNow;

            if (profile.Id == 0)
            {
                profile.CreatedAt = DateTime.UtcNow;
                await db.InsertAsync(profile);
                Console.WriteLine($"Created user profile: {profile.Name} (ID: {profile.Id})");
            }
            else
            {
                await db.UpdateAsync(profile);
                Console.WriteLine($"Updated user profile: {profile.Name} (ID: {profile.Id})");
            }

            return profile;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving user profile: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    #endregion

    #region Avatar Configuration Methods

    public async Task<AvatarConfiguration?> GetAvatarConfigurationAsync(int id)
    {
        try
        {
            var db = await GetConnectionAsync();
            return await db.Table<AvatarConfiguration>()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting avatar config: {ex.Message}");
            return null;
        }
    }

    public async Task<AvatarConfiguration> SaveAvatarConfigurationAsync(AvatarConfiguration config)
    {
        try
        {
            var db = await GetConnectionAsync();
            config.UpdatedAt = DateTime.UtcNow;

            if (config.Id == 0)
            {
                config.CreatedAt = DateTime.UtcNow;
                await db.InsertAsync(config);
                Console.WriteLine($"Created avatar config: {config.Gender} (ID: {config.Id})");
            }
            else
            {
                await db.UpdateAsync(config);
                Console.WriteLine($"Updated avatar config: {config.Gender} (ID: {config.Id})");
            }

            return config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving avatar config: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<AvatarConfiguration?> GetUserAvatarAsync()
    {
        try
        {
            var profile = await GetUserProfileAsync();
            if (profile?.AvatarConfigId == null)
            {
                Console.WriteLine("User has no avatar yet");
                return null;
            }

            var avatar = await GetAvatarConfigurationAsync(profile.AvatarConfigId.Value);
            if (avatar != null)
            {
                Console.WriteLine($"Loaded user avatar: {avatar.Gender}, {avatar.HairStyle}-{avatar.HairColor}");
            }
            
            return avatar;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user avatar: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Security Credential Methods

    /// <summary>
    /// Get security credential from database
    /// </summary>
    public async Task<SecurityCredential?> GetSecurityCredentialAsync()
    {
        try
        {
            var db = await GetConnectionAsync();
            var credentials = await db.Table<SecurityCredential>()
                .Where(c => c.IsEnabled)
                .ToListAsync();
            
            return credentials.FirstOrDefault();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetSecurityCredentialAsync Error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Save security credential (creates new or updates existing)
    /// </summary>
    public async Task<bool> SaveSecurityCredentialAsync(SecurityCredential credential)
    {
        try
        {
            var db = await GetConnectionAsync();
            
            // Delete any existing credentials first
            await db.ExecuteAsync("DELETE FROM SecurityCredential");
            
            // Insert new credential
            credential.UpdatedAt = DateTime.UtcNow;
            int result = await db.InsertAsync(credential);
            
            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SaveSecurityCredentialAsync Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Delete security credential (disable security)
    /// </summary>
    public async Task<bool> DeleteSecurityCredentialAsync()
    {
        try
        {
            var db = await GetConnectionAsync();
            int result = await db.ExecuteAsync("DELETE FROM SecurityCredential");
            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteSecurityCredentialAsync Error: {ex.Message}");
            return false;
        }
    }

    #endregion
}