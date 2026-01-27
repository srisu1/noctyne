using SQLite;
using MoodJournal.Models;

namespace MoodJournal.Services;

public class DatabaseService : Services.Interfaces.IDatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "moodjournal.db3");
        Console.WriteLine($"Database path: {_dbPath}");
    }

    public async Task InitializeAsync()
    {
        if (_database != null) return;

        try
        {
            _database = new SQLiteAsyncConnection(_dbPath);
            
            // Note: If table creation fails, use the SQL file to create tables manually
            try
            {
                await _database.CreateTableAsync<AppSetting>();
                await _database.CreateTableAsync<UserProfile>();
                await _database.CreateTableAsync<AvatarConfiguration>();
                await _database.CreateTableAsync<SecurityCredential>();
                await _database.CreateTableAsync<JournalEntry>();
                await _database.CreateTableAsync<Tag>();
                await _database.CreateTableAsync<EntryTag>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Table creation failed: {ex.Message}");
                Console.WriteLine("Use DATABASE_SETUP_SQL.sql to create tables manually");
            }
            
            await SeedDefaultSettingsAsync();
            Console.WriteLine("Database initialized!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database error: {ex.Message}");
            throw;
        }
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database == null) await InitializeAsync();
        return _database!;
    }

    private async Task SeedDefaultSettingsAsync()
    {
        var count = await _database!.Table<AppSetting>().CountAsync();
        if (count > 0) return;

        var settings = new List<AppSetting>
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
        await _database.InsertAllAsync(settings);
    }
    
    // In DatabaseService class
    public async Task<AvatarConfiguration?> GetUserAvatarByIdAsync(int avatarConfigId)
    {
        var db = await GetConnectionAsync();
        return await db.Table<AvatarConfiguration>()
            .Where(a => a.Id == avatarConfigId)
            .FirstOrDefaultAsync();
    }



    #region Settings

    public async Task<string?> GetSettingAsync(string key)
    {
        var db = await GetConnectionAsync();
        var setting = await db.Table<AppSetting>()
            .Where(s => s.SettingKey == key)
            .FirstOrDefaultAsync();
        return setting?.SettingValue;
    }

    public async Task SetSettingAsync(string key, string value)
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
        }
        else
        {
            await db.InsertAsync(new AppSetting { SettingKey = key, SettingValue = value });
        }
    }

    public async Task<bool> GetBoolSettingAsync(string key, bool defaultValue = false)
    {
        var value = await GetSettingAsync(key);
        return string.IsNullOrEmpty(value) ? defaultValue : bool.TryParse(value, out var result) && result;
    }

    public async Task SetBoolSettingAsync(string key, bool value)
    {
        await SetSettingAsync(key, value.ToString().ToLower());
    }

    #endregion

    #region User Profile

    public async Task<UserProfile?> GetUserProfileAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<UserProfile>().FirstOrDefaultAsync();
    }

    public async Task<UserProfile> SaveUserProfileAsync(UserProfile profile)
    {
        var db = await GetConnectionAsync();
        profile.UpdatedAt = DateTime.UtcNow;

        if (profile.Id == 0)
        {
            profile.CreatedAt = DateTime.UtcNow;
            await db.InsertAsync(profile);
        }
        else
        {
            await db.UpdateAsync(profile);
        }
        return profile;
    }

    #endregion

    #region Avatar

    public async Task<AvatarConfiguration?> GetAvatarConfigurationAsync(int id)
    {
        var db = await GetConnectionAsync();
        return await db.Table<AvatarConfiguration>()
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<AvatarConfiguration> SaveAvatarConfigurationAsync(AvatarConfiguration config)
    {
        var db = await GetConnectionAsync();
        config.UpdatedAt = DateTime.UtcNow;

        if (config.Id == 0)
        {
            config.CreatedAt = DateTime.UtcNow;
            await db.InsertAsync(config);
        }
        else
        {
            await db.UpdateAsync(config);
        }
        return config;
    }

    public async Task<AvatarConfiguration?> GetUserAvatarAsync()
    {
        var profile = await GetUserProfileAsync();
        if (profile?.AvatarConfigId == null) return null;
        return await GetAvatarConfigurationAsync(profile.AvatarConfigId.Value);
    }

    #endregion

    #region Security

    public async Task<SecurityCredential?> GetSecurityCredentialAsync()
    {
        var db = await GetConnectionAsync();
        var creds = await db.Table<SecurityCredential>()
            .Where(c => c.IsEnabled)
            .ToListAsync();
        return creds.FirstOrDefault();
    }

    public async Task<bool> SaveSecurityCredentialAsync(SecurityCredential credential)
    {
        var db = await GetConnectionAsync();
        await db.ExecuteAsync("DELETE FROM SecurityCredential");
        credential.UpdatedAt = DateTime.UtcNow;
        int result = await db.InsertAsync(credential);
        return result > 0;
    }

    public async Task<bool> DeleteSecurityCredentialAsync()
    {
        var db = await GetConnectionAsync();
        int result = await db.ExecuteAsync("DELETE FROM SecurityCredential");
        return result > 0;
    }

    #endregion

    #region Journal Entries

    public async Task<JournalEntry?> GetJournalEntryByIdAsync(int id)
    {
        await InitializeAsync();
        return await _database!.Table<JournalEntry>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<JournalEntry?> GetJournalEntryByDateAsync(string date)
    {
        await InitializeAsync();
        return await _database!.Table<JournalEntry>()
            .Where(e => e.EntryDate == date)
            .FirstOrDefaultAsync();
    }

    public async Task<List<JournalEntry>> GetAllJournalEntriesAsync()
    {
        await InitializeAsync();
        return await _database!.Table<JournalEntry>()
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> GetJournalEntriesByDateRangeAsync(string startDate, string endDate)
    {
        await InitializeAsync();
        // FIXED: Use string comparison instead of >= and <=
        var allEntries = await _database!.Table<JournalEntry>().ToListAsync();
        return allEntries
            .Where(e => string.Compare(e.EntryDate, startDate) >= 0 && string.Compare(e.EntryDate, endDate) <= 0)
            .OrderByDescending(e => e.EntryDate)
            .ToList();
    }

    public async Task<List<JournalEntry>> GetJournalEntriesByPrimaryMoodAsync(string mood)
    {
        await InitializeAsync();
        return await _database!.Table<JournalEntry>()
            .Where(e => e.PrimaryMood == mood)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> SearchJournalEntriesAsync(string searchTerm)
    {
        await InitializeAsync();
        var allEntries = await _database!.Table<JournalEntry>().ToListAsync();
        return allEntries.Where(e =>
            (!string.IsNullOrEmpty(e.Title) && e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(e.Content) && e.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        ).OrderByDescending(e => e.EntryDate).ToList();
    }

    public async Task<JournalEntry?> CreateJournalEntryAsync(JournalEntry entry)
    {
        await InitializeAsync();
        await _database!.InsertAsync(entry);
        return entry;
    }

    public async Task<bool> UpdateJournalEntryAsync(JournalEntry entry)
    {
        await InitializeAsync();
        await _database!.UpdateAsync(entry);
        return true;
    }

    public async Task<bool> DeleteJournalEntryAsync(int id)
    {
        await InitializeAsync();
        await _database!.DeleteAsync<JournalEntry>(id);
        return true;
    }

    public async Task<bool> JournalEntryExistsForDateAsync(string date)
    {
        await InitializeAsync();
        var count = await _database!.Table<JournalEntry>()
            .Where(e => e.EntryDate == date)
            .CountAsync();
        return count > 0;
    }

    public async Task<int> GetJournalEntryCountAsync()
    {
        await InitializeAsync();
        return await _database!.Table<JournalEntry>().CountAsync();
    }

    public async Task<int> GetTotalWordCountAsync()
    {
        await InitializeAsync();
        var entries = await _database!.Table<JournalEntry>().ToListAsync();
        return entries.Sum(e => e.WordCount);
    }

    #endregion

    #region Tags

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        await InitializeAsync();
        return await _database!.Table<Tag>()
            .OrderByDescending(t => t.UsageCount)
            .ToListAsync();
    }

    public async Task<Tag?> GetTagByIdAsync(int id)
    {
        await InitializeAsync();
        return await _database!.Table<Tag>()
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        await InitializeAsync();
        return await _database!.Table<Tag>()
            .Where(t => t.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        await InitializeAsync();
        await _database!.InsertAsync(tag);
        return tag;
    }

    public async Task<bool> UpdateTagAsync(Tag tag)
    {
        await InitializeAsync();
        await _database!.UpdateAsync(tag);
        return true;
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        await InitializeAsync();
        await _database!.DeleteAsync<Tag>(id);
        return true;
    }

    public async Task<List<Tag>> GetMostUsedTagsAsync(int count)
    {
        await InitializeAsync();
        return await _database!.Table<Tag>()
            .OrderByDescending(t => t.UsageCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Tag>> SearchTagsAsync(string searchTerm)
    {
        await InitializeAsync();
        var allTags = await _database!.Table<Tag>().ToListAsync();
        return allTags.Where(t => t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.UsageCount)
            .ToList();
    }

    #endregion

    #region Entry-Tag Relationships

    public async Task<List<Tag>> GetTagsForEntryAsync(int entryId)
    {
        await InitializeAsync();
        var entryTags = await _database!.Table<EntryTag>()
            .Where(et => et.EntryId == entryId)
            .ToListAsync();
        
        var tags = new List<Tag>();
        foreach (var entryTag in entryTags)
        {
            var tag = await GetTagByIdAsync(entryTag.TagId);
            if (tag != null) tags.Add(tag);
        }
        return tags;
    }

    public async Task<List<EntryTag>> GetEntryTagsAsync(int entryId)
    {
        await InitializeAsync();
        return await _database!.Table<EntryTag>()
            .Where(et => et.EntryId == entryId)
            .ToListAsync();
    }

    public async Task<bool> CreateEntryTagAsync(EntryTag entryTag)
    {
        await InitializeAsync();
        await _database!.InsertAsync(entryTag);
        return true;
    }

    public async Task<bool> DeleteEntryTagAsync(int entryId, int tagId)
    {
        await InitializeAsync();
        var entryTag = await _database!.Table<EntryTag>()
            .Where(et => et.EntryId == entryId && et.TagId == tagId)
            .FirstOrDefaultAsync();
        
        if (entryTag != null)
        {
            await _database.DeleteAsync(entryTag);
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteAllEntryTagsAsync(int entryId)
    {
        await InitializeAsync();
        var entryTags = await _database!.Table<EntryTag>()
            .Where(et => et.EntryId == entryId)
            .ToListAsync();
        
        foreach (var entryTag in entryTags)
        {
            await _database.DeleteAsync(entryTag);
        }
        return true;
    }

    #endregion
}