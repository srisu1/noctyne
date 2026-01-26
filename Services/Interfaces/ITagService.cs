using MoodJournal.Models;

namespace MoodJournal.Services.Interfaces;

/// <summary>
/// Service interface for tag operations
/// Handles CRUD operations for tags and tag statistics
/// </summary>
public interface ITagService
{

    // Gets all tags ordered by usage count (most used first)
    Task<List<Tag>> GetAllTagsAsync();


    // Gets a tag by ID
    Task<Tag?> GetTagByIdAsync(int id);


    // Gets a tag by name (case-insensitive)
    Task<Tag?> GetTagByNameAsync(string name);


    //Creates a new tag or returns existing if name already exists
    //Tag names are stored in lowercase
    Task<Tag> CreateOrGetTagAsync(string name);


    // Deletes a tag and all its relationships
    Task<bool> DeleteTagAsync(int id);


    // Gets most frequently used tags
    Task<List<Tag>> GetMostUsedTagsAsync(int count = 10);

  
    // Gets suggested tags based on existing tags
    // Returns tags ordered by usage count
    Task<List<Tag>> GetSuggestedTagsAsync(int limit = 5);


    // Searches tags by name (partial match)
    Task<List<Tag>> SearchTagsAsync(string searchTerm);


    // Increments usage count for a tag
    // Updates LastUsedAt timestamp
    Task<bool> IncrementTagUsageAsync(int tagId);

    
    // Decrements usage count for a tag
    //Updates LastUsedAt timestamp if still in use
    Task<bool> DecrementTagUsageAsync(int tagId);
}