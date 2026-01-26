using System.Security.Cryptography;
using System.Text;
using MoodJournal.Models;
using MoodJournal.Services.Interfaces;

namespace MoodJournal.Services;

public class SecurityService : ISecurityService
{
    private readonly IDatabaseService _databaseService;

    public SecurityService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

  
    //Hash a credential (PIN or Password) with a random salt using SHA256
    public (string hash, string salt) HashCredential(string credential)
    {
        // Generate random salt
        byte[] saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);

        // Hash credential with salt
        string hash = HashWithSalt(credential, salt);

        return (hash, salt);
    }

  
    //Verify a credential against stored hash and salt
    public bool VerifyCredential(string credential, string hash, string salt)
    {
        string computedHash = HashWithSalt(credential, salt);
        return computedHash == hash;
    }

    
    //Set up security for the first time
    public async Task<bool> SetupSecurityAsync(string credentialType, string credential)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(credential))
                return false;

            if (credentialType == "pin" && credential.Length != 6)
                return false;

            if (credentialType == "password" && credential.Length < 6)
                return false;

            // Hash the credential
            var (hash, salt) = HashCredential(credential);

            // Create credential object
            var securityCredential = new SecurityCredential
            {
                CredentialType = credentialType,
                HashedValue = hash,
                Salt = salt,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save to database
            bool saved = await _databaseService.SaveSecurityCredentialAsync(securityCredential);

            if (saved)
            {
                // Update SecurityEnabled setting
                await _databaseService.SetBoolSettingAsync(SettingKeys.SecurityEnabled, true);
            }

            return saved;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SetupSecurityAsync Error: {ex.Message}");
            return false;
        }
    }

 
    //Verify entered credential
    public async Task<bool> VerifySecurityAsync(string credential)
    {
        try
        {
            var storedCredential = await _databaseService.GetSecurityCredentialAsync();
            
            if (storedCredential == null || !storedCredential.IsEnabled)
                return false;

            return VerifyCredential(credential, storedCredential.HashedValue, storedCredential.Salt);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"VerifySecurityAsync Error: {ex.Message}");
            return false;
        }
    }

  
    // Check if security is enabled
    public async Task<bool> IsSecurityEnabledAsync()
    {
        try
        {
            bool settingEnabled = await _databaseService.GetBoolSettingAsync(SettingKeys.SecurityEnabled);
            
            if (!settingEnabled)
                return false;

            var credential = await _databaseService.GetSecurityCredentialAsync();
            return credential != null && credential.IsEnabled;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"IsSecurityEnabledAsync Error: {ex.Message}");
            return false;
        }
    }


    //Get credential type
    public async Task<string?> GetCredentialTypeAsync()
    {
        try
        {
            var credential = await _databaseService.GetSecurityCredentialAsync();
            return credential?.CredentialType;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetCredentialTypeAsync Error: {ex.Message}");
            return null;
        }
    }

    
    //Disable security
    public async Task<bool> DisableSecurityAsync()
    {
        try
        {
            bool deleted = await _databaseService.DeleteSecurityCredentialAsync();
            
            if (deleted)
            {
                await _databaseService.SetBoolSettingAsync(SettingKeys.SecurityEnabled, false);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DisableSecurityAsync Error: {ex.Message}");
            return false;
        }
    }


    //Hash a credential with a salt using SHA256
    private string HashWithSalt(string credential, string salt)
    {
        // Combine credential and salt
        string combined = credential + salt;
        
        // Hash with SHA256
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return Convert.ToBase64String(hashBytes);
        }
    }
}