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
}