using MoodJournal.Models;
using MoodJournal.Services.Interfaces;
using System.Text.RegularExpressions;

namespace MoodJournal.Services;

/// <summary>
/// Service for journal entry operations with mood tracking and tag management
/// Implements business logic for CRUD operations, validation, and statistics
/// </summary>
public class JournalService : IJournalService
{
    private readonly IDatabaseService _databaseService;
    private readonly ITagService _tagService;

    public JournalService(IDatabaseService databaseService, ITagService tagService)
    {
        _databaseService = databaseService;
        _tagService = tagService;
    }

    #region Entry CRUD Operations

    public async Task<JournalEntry?> GetEntryByIdAsync(int id)
    {
        try
        {
            return await _databaseService.GetJournalEntryByIdAsync(id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEntryByIdAsync Error: {ex.Message}");
            return null;
        }
    }

    public async Task<JournalEntry?> GetEntryByDateAsync(string date)
    {
        try
        {
            return await _databaseService.GetJournalEntryByDateAsync(date);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEntryByDateAsync Error: {ex.Message}");
            return null;
        }
    }

    public async Task<List<JournalEntry>> GetAllEntriesAsync()
    {
        try
        {
            return await _databaseService.GetAllJournalEntriesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAllEntriesAsync Error: {ex.Message}");
            return new List<JournalEntry>();
        }
    }

    public async Task<List<JournalEntry>> GetEntriesByDateRangeAsync(string startDate, string endDate)
    {
        try
        {
            return await _databaseService.GetJournalEntriesByDateRangeAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEntriesByDateRangeAsync Error: {ex.Message}");
            return new List<JournalEntry>();
        }
    }

    public async Task<List<JournalEntry>> GetEntriesByPrimaryMoodAsync(string mood)
    {
        try
        {
            return await _databaseService.GetJournalEntriesByPrimaryMoodAsync(mood);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEntriesByPrimaryMoodAsync Error: {ex.Message}");
            return new List<JournalEntry>();
        }
    }

    public async Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm)
    {
        try
        {
            return await _databaseService.SearchJournalEntriesAsync(searchTerm);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SearchEntriesAsync Error: {ex.Message}");
            return new List<JournalEntry>();
        }
    }

    public async Task<JournalEntry?> CreateEntryAsync(JournalEntry entry)
    {
        try
        {
            // Validation: Primary mood is required
            if (string.IsNullOrWhiteSpace(entry.PrimaryMood))
            {
                System.Diagnostics.Debug.WriteLine("CreateEntryAsync: Primary mood is required");
                return null;
            }

            // Validation: Primary mood must be valid
            if (!Moods.IsValid(entry.PrimaryMood))
            {
                System.Diagnostics.Debug.WriteLine($"CreateEntryAsync: Invalid mood '{entry.PrimaryMood}'");
                return null;
            }

            // Validation: Content cannot be empty
            if (string.IsNullOrWhiteSpace(entry.Content))
            {
                System.Diagnostics.Debug.WriteLine("CreateEntryAsync: Content is required");
                return null;
            }

            // Validation: Only one entry per day
            var existingEntry = await _databaseService.GetJournalEntryByDateAsync(entry.EntryDate);
            if (existingEntry != null)
            {
                System.Diagnostics.Debug.WriteLine($"CreateEntryAsync: Entry already exists for {entry.EntryDate}");
                return null;
            }

            // Calculate word and character counts
            entry.WordCount = CalculateWordCount(entry.Content);
            entry.CharacterCount = CalculateCharacterCount(entry.Content);

            // Set timestamps
            entry.CreatedAt = DateTime.UtcNow;
            entry.UpdatedAt = DateTime.UtcNow;

            // Create entry
            var createdEntry = await _databaseService.CreateJournalEntryAsync(entry);

            return createdEntry;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CreateEntryAsync Error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateEntryAsync(JournalEntry entry)
    {
        try
        {
            // Validation: Primary mood is required
            if (string.IsNullOrWhiteSpace(entry.PrimaryMood))
            {
                System.Diagnostics.Debug.WriteLine("UpdateEntryAsync: Primary mood is required");
                return false;
            }

            // Validation: Primary mood must be valid
            if (!Moods.IsValid(entry.PrimaryMood))
            {
                System.Diagnostics.Debug.WriteLine($"UpdateEntryAsync: Invalid mood '{entry.PrimaryMood}'");
                return false;
            }

            // Validation: Content cannot be empty
            if (string.IsNullOrWhiteSpace(entry.Content))
            {
                System.Diagnostics.Debug.WriteLine("UpdateEntryAsync: Content is required");
                return false;
            }

            // Recalculate word and character counts
            entry.WordCount = CalculateWordCount(entry.Content);
            entry.CharacterCount = CalculateCharacterCount(entry.Content);

            // Update timestamp
            entry.UpdatedAt = DateTime.UtcNow;

            return await _databaseService.UpdateJournalEntryAsync(entry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateEntryAsync Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteEntryAsync(int id)
    {
        try
        {
            // Delete all entry-tag relationships first
            await _databaseService.DeleteAllEntryTagsAsync(id);

            // Delete the entry
            return await _databaseService.DeleteJournalEntryAsync(id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteEntryAsync Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EntryExistsForDateAsync(string date)
    {
        try
        {
            return await _databaseService.JournalEntryExistsForDateAsync(date);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EntryExistsForDateAsync Error: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Tag Operations

    public async Task<List<Tag>> GetTagsForEntryAsync(int entryId)
    {
        try
        {
            return await _databaseService.GetTagsForEntryAsync(entryId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetTagsForEntryAsync Error: {ex.Message}");
            return new List<Tag>();
        }
    }

    public async Task<bool> AddTagsToEntryAsync(int entryId, List<string> tagNames)
    {
        try
        {
            foreach (var tagName in tagNames)
            {
                // Normalize tag name (lowercase, trim)
                var normalizedName = tagName.Trim().ToLower();
                
                if (string.IsNullOrWhiteSpace(normalizedName))
                    continue;

                // Get or create tag
                var tag = await _tagService.CreateOrGetTagAsync(normalizedName);

                // Create entry-tag relationship
                var entryTag = new EntryTag
                {
                    EntryId = entryId,
                    TagId = tag.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await _databaseService.CreateEntryTagAsync(entryTag);

                // Increment tag usage
                await _tagService.IncrementTagUsageAsync(tag.Id);
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AddTagsToEntryAsync Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RemoveTagFromEntryAsync(int entryId, int tagId)
    {
        try
        {
            // Delete entry-tag relationship
            var success = await _databaseService.DeleteEntryTagAsync(entryId, tagId);

            if (success)
            {
                // Decrement tag usage
                await _tagService.DecrementTagUsageAsync(tagId);
            }

            return success;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RemoveTagFromEntryAsync Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SetEntryTagsAsync(int entryId, List<string> tagNames)
    {
        try
        {
            // Get current tags
            var currentTags = await GetTagsForEntryAsync(entryId);

            // Remove all current entry-tag relationships
            foreach (var tag in currentTags)
            {
                await _databaseService.DeleteEntryTagAsync(entryId, tag.Id);
                await _tagService.DecrementTagUsageAsync(tag.Id);
            }

            // Add new tags
            return await AddTagsToEntryAsync(entryId, tagNames);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SetEntryTagsAsync Error: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Statistics

    public async Task<int> GetEntryCountAsync()
    {
        try
        {
            return await _databaseService.GetJournalEntryCountAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEntryCountAsync Error: {ex.Message}");
            return 0;
        }
    }

    public async Task<int> GetTotalWordCountAsync()
    {
        try
        {
            return await _databaseService.GetTotalWordCountAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetTotalWordCountAsync Error: {ex.Message}");
            return 0;
        }
    }

    public async Task<Dictionary<string, int>> GetEntriesByMonthAsync()
    {
        try
        {
            var entries = await GetAllEntriesAsync();
            var groupedByMonth = entries
                .GroupBy(e => e.EntryDate.Substring(0, 7)) // Group by YYYY-MM
                .ToDictionary(g => g.Key, g => g.Count());

            return groupedByMonth;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEntriesByMonthAsync Error: {ex.Message}");
            return new Dictionary<string, int>();
        }
    }




    //Gets total number of entries (alias for GetEntryCountAsync)

    public async Task<int> GetTotalEntriesCountAsync()
    {
        return await GetEntryCountAsync();
    }

 
    // Gets count of entries created this month

    public async Task<int> GetEntriesCountThisMonthAsync()
    {
        try
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
            var endOfMonth = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)).ToString("yyyy-MM-dd");
            
            var entries = await GetEntriesByDateRangeAsync(startOfMonth, endOfMonth);
            return entries.Count;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEntriesCountThisMonthAsync Error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Gets average word count per entry
    /// </summary>
    public async Task<int> GetAverageWordCountAsync()
    {
        try
        {
            var entries = await GetAllEntriesAsync();
            
            if (entries.Count == 0)
                return 0;
            
            var totalWords = entries.Sum(e => e.WordCount);
            return totalWords / entries.Count;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAverageWordCountAsync Error: {ex.Message}");
            return 0;
        }
    }

  
    // Gets the most frequently used primary mood

    public async Task<string> GetMostFrequentMoodAsync()
    {
        try
        {
            var entries = await GetAllEntriesAsync();
            
            if (entries.Count == 0)
                return string.Empty;
            
            var moodCounts = entries
                .Where(e => !string.IsNullOrEmpty(e.PrimaryMood))
                .GroupBy(e => e.PrimaryMood)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();
            
            return moodCounts?.Key ?? string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetMostFrequentMoodAsync Error: {ex.Message}");
            return string.Empty;
        }
    }


    // Gets recent entries (ordered by created date, newest first)
 
    public async Task<List<JournalEntry>> GetRecentEntriesAsync(int count)
    {
        try
        {
            var allEntries = await GetAllEntriesAsync();
            return allEntries.OrderByDescending(e => e.CreatedAt).Take(count).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetRecentEntriesAsync Error: {ex.Message}");
            return new List<JournalEntry>();
        }
    }

    #endregion

    #region Helper Methods


    // Calculates word count from HTML content
    //Strips HTML tags and counts words

    private int CalculateWordCount(string htmlContent)
    {
        try
        {
            // Remove HTML tags
            var text = Regex.Replace(htmlContent, "<.*?>", string.Empty);
            
            // Remove extra whitespace
            text = Regex.Replace(text, @"\s+", " ").Trim();
            
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            // Split by whitespace and count
            var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }
        catch
        {
            return 0;
        }
    }


    //Calculates character count from HTML content
    //Strips HTML tags and counts characters (excluding whitespace)
    
    private int CalculateCharacterCount(string htmlContent)
    {
        try
        {
            // Remove HTML tags
            var text = Regex.Replace(htmlContent, "<.*?>", string.Empty);
            
            // Remove whitespace and count
            text = Regex.Replace(text, @"\s", string.Empty);
            
            return text.Length;
        }
        catch
        {
            return 0;
        }
    }

    #endregion
}