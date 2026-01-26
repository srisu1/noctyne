using MoodJournal.Models;

namespace MoodJournal.Services.Interfaces;

public interface IDatabaseService
{
    Task InitializeAsync();
    Task<SQLite.SQLiteAsyncConnection> GetConnectionAsync();
    
    // Settings
    Task<string?> GetSettingAsync(string key);
    Task SetSettingAsync(string key, string value);
    Task<bool> GetBoolSettingAsync(string key, bool defaultValue = false);
    Task SetBoolSettingAsync(string key, bool value);
    
    // User Profile
    Task<UserProfile?> GetUserProfileAsync();
    Task<UserProfile> SaveUserProfileAsync(UserProfile profile);
    
    // Avatar Configuration
    Task<AvatarConfiguration?> GetAvatarConfigurationAsync(int id);
    Task<AvatarConfiguration> SaveAvatarConfigurationAsync(AvatarConfiguration config);
    Task<AvatarConfiguration?> GetUserAvatarAsync();
    
    // Security Credential
    Task<SecurityCredential?> GetSecurityCredentialAsync();
    Task<bool> SaveSecurityCredentialAsync(SecurityCredential credential);
    Task<bool> DeleteSecurityCredentialAsync();
    
    

    #region Journal Entries

    Task<JournalEntry?> GetJournalEntryByIdAsync(int id);
    Task<JournalEntry?> GetJournalEntryByDateAsync(string date);
    Task<List<JournalEntry>> GetAllJournalEntriesAsync();
    Task<List<JournalEntry>> GetJournalEntriesByDateRangeAsync(string startDate, string endDate);
    Task<List<JournalEntry>> GetJournalEntriesByPrimaryMoodAsync(string mood);
    Task<List<JournalEntry>> SearchJournalEntriesAsync(string searchTerm);
    Task<JournalEntry?> CreateJournalEntryAsync(JournalEntry entry);
    Task<bool> UpdateJournalEntryAsync(JournalEntry entry);
    Task<bool> DeleteJournalEntryAsync(int id);
    Task<bool> JournalEntryExistsForDateAsync(string date);
    Task<int> GetJournalEntryCountAsync();
    Task<int> GetTotalWordCountAsync();

    #endregion

    #region Tags

    Task<List<Tag>> GetAllTagsAsync();
    Task<Tag?> GetTagByIdAsync(int id);
    Task<Tag?> GetTagByNameAsync(string name);
    Task<Tag> CreateTagAsync(Tag tag);
    Task<bool> UpdateTagAsync(Tag tag);
    Task<bool> DeleteTagAsync(int id);
    Task<List<Tag>> GetMostUsedTagsAsync(int count);
    Task<List<Tag>> SearchTagsAsync(string searchTerm);

    #endregion

    #region Entry-Tag Relationships

    Task<List<Tag>> GetTagsForEntryAsync(int entryId);
    Task<List<EntryTag>> GetEntryTagsAsync(int entryId);
    Task<bool> CreateEntryTagAsync(EntryTag entryTag);
    Task<bool> DeleteEntryTagAsync(int entryId, int tagId);
    Task<bool> DeleteAllEntryTagsAsync(int entryId);

    #endregion
}