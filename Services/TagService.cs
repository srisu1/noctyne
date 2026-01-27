using MoodJournal.Models;
using MoodJournal.Services.Interfaces;

namespace MoodJournal.Services;

/// <summary>
/// Service for tag operations
/// Handles tag creation, retrieval, usage tracking, and suggestions
/// </summary>
public class TagService : ITagService
{
    private readonly IDatabaseService _databaseService;

    public TagService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        try
        {
            return await _databaseService.GetAllTagsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAllTagsAsync Error: {ex.Message}");
            return new List<Tag>();
        }
    }

    public async Task<Tag?> GetTagByIdAsync(int id)
    {
        try
        {
            return await _databaseService.GetTagByIdAsync(id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetTagByIdAsync Error: {ex.Message}");
            return null;
        }
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        try
        {
            // Normalize name (lowercase, trim)
            var normalizedName = name.Trim().ToLower();
            return await _databaseService.GetTagByNameAsync(normalizedName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetTagByNameAsync Error: {ex.Message}");
            return null;
        }
    }

    public async Task<Tag> CreateOrGetTagAsync(string name)
    {
        try
        {
            // Normalize name (lowercase, trim)
            var normalizedName = name.Trim().ToLower();

            // Check if tag already exists
            var existingTag = await GetTagByNameAsync(normalizedName);
            if (existingTag != null)
            {
                return existingTag;
            }

            // Create new tag
            var newTag = new Tag
            {
                Name = normalizedName,
                UsageCount = 0,
                CreatedAt = DateTime.UtcNow,
                LastUsedAt = DateTime.UtcNow
            };

            return await _databaseService.CreateTagAsync(newTag);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CreateOrGetTagAsync Error: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        try
        {
            return await _databaseService.DeleteTagAsync(id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteTagAsync Error: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Tag>> GetMostUsedTagsAsync(int count = 10)
    {
        try
        {
            return await _databaseService.GetMostUsedTagsAsync(count);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetMostUsedTagsAsync Error: {ex.Message}");
            return new List<Tag>();
        }
    }

    public async Task<List<Tag>> GetSuggestedTagsAsync(int limit = 5)
    {
        try
        {
            // Return most used tags as suggestions
            return await GetMostUsedTagsAsync(limit);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetSuggestedTagsAsync Error: {ex.Message}");
            return new List<Tag>();
        }
    }

    public async Task<List<Tag>> SearchTagsAsync(string searchTerm)
    {
        try
        {
            return await _databaseService.SearchTagsAsync(searchTerm);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SearchTagsAsync Error: {ex.Message}");
            return new List<Tag>();
        }
    }

    public async Task<bool> IncrementTagUsageAsync(int tagId)
    {
        try
        {
            var tag = await GetTagByIdAsync(tagId);
            if (tag == null)
            {
                System.Diagnostics.Debug.WriteLine($"IncrementTagUsageAsync: Tag {tagId} not found");
                return false;
            }

            tag.UsageCount++;
            tag.LastUsedAt = DateTime.UtcNow;

            return await _databaseService.UpdateTagAsync(tag);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"IncrementTagUsageAsync Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DecrementTagUsageAsync(int tagId)
    {
        try
        {
            var tag = await GetTagByIdAsync(tagId);
            if (tag == null)
            {
                System.Diagnostics.Debug.WriteLine($"DecrementTagUsageAsync: Tag {tagId} not found");
                return false;
            }

            // Don't go below zero
            if (tag.UsageCount > 0)
            {
                tag.UsageCount--;
            }

            // Only update LastUsedAt if still in use
            if (tag.UsageCount > 0)
            {
                tag.LastUsedAt = DateTime.UtcNow;
            }

            return await _databaseService.UpdateTagAsync(tag);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DecrementTagUsageAsync Error: {ex.Message}");
            return false;
        }
    }

    // ===== NEW DASHBOARD METHOD =====

    /// <summary>
    /// Gets the name of the most used tag
    /// </summary>
    public async Task<string> GetMostUsedTagAsync()
    {
        try
        {
            var allTags = await GetAllTagsAsync();
            
            if (allTags.Count == 0)
                return string.Empty;
            
            var mostUsedTag = allTags
                .OrderByDescending(t => t.UsageCount)
                .FirstOrDefault();
            
            return mostUsedTag?.Name ?? string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetMostUsedTagAsync Error: {ex.Message}");
            return string.Empty;
        }
    }
}