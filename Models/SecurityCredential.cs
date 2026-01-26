using SQLite;

namespace MoodJournal.Models;

[Table("SecurityCredential")]
public class SecurityCredential
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [NotNull]
    public string CredentialType { get; set; } = "pin"; // "pin" or "password"
    
    [NotNull]
    public string HashedValue { get; set; } = string.Empty;
    
    [NotNull]
    public string Salt { get; set; } = string.Empty;
    
    [NotNull]
    public bool IsEnabled { get; set; } = true;
    
    [NotNull]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [NotNull]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}