namespace MoodJournal.Services.Interfaces;

public interface ISecurityService
{
    // Hash a PIN or password with a salt
    (string hash, string salt) HashCredential(string credential);
    
  
    //Verify a PIN or password against stored hash

    bool VerifyCredential(string credential, string hash, string salt);
    
    
    //Set up security for the first time (PIN or Password)

    Task<bool> SetupSecurityAsync(string credentialType, string credential);
    
   
    //Verify entered PIN/Password
    
    Task<bool> VerifySecurityAsync(string credential);
    
   
    //Check if security is enabled
    Task<bool> IsSecurityEnabledAsync();
    

    // Get credential type ("pin" or "password")
    Task<string?> GetCredentialTypeAsync();
    
 
    //Disable security (remove credentials)
    Task<bool> DisableSecurityAsync();
}