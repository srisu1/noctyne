using MoodJournal.Models;

namespace MoodJournal.Services.Interfaces;

/// <summary>
/// Service interface for journal entry operations
/// Handles CRUD operations for journal entries with mood and tag management
/// </summary>
public interface IJournalService
{
    #region Entry CRUD Operations
    
    // Gets a journal entry by ID
    Task<JournalEntry?> GetEntryByIdAsync(int id);
    
    // Gets a journal entry for a specific date
    Task<JournalEntry?> GetEntryByDateAsync(string date);
    
    // Gets all journal entries ordered by date (newest first)
    Task<List<JournalEntry>> GetAllEntriesAsync();
    
    // Gets journal entries within a date range
    Task<List<JournalEntry>> GetEntriesByDateRangeAsync(string startDate, string endDate);
    
    // Gets entries filtered by primary mood
    Task<List<JournalEntry>> GetEntriesByPrimaryMoodAsync(string mood);
    
    // Searches entries by title or content
    Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm);
    
    // Creates a new journal entry
    Task<JournalEntry?> CreateEntryAsync(JournalEntry entry);
    
    // Updates an existing journal entry
    Task<bool> UpdateEntryAsync(JournalEntry entry);
    
    // Deletes a journal entry by ID and also removes associated entry-tag relationships
    Task<bool> DeleteEntryAsync(int id);
    
    // Checks if an entry exists for a specific date
    Task<bool> EntryExistsForDateAsync(string date);
    
    #endregion
    
    #region Tag Operations
    
    // Gets all tags for a specific entry
    Task<List<Tag>> GetTagsForEntryAsync(int entryId);
    
    // Adds tags to an entry - creates new tags if they don't exist, updates usage count
    Task<bool> AddTagsToEntryAsync(int entryId, List<string> tagNames);
    
    // Removes a tag from an entry and updates tag usage count
    Task<bool> RemoveTagFromEntryAsync(int entryId, int tagId);
    
    // Replaces all tags for an entry - removes old tags, adds new ones, updates usage counts
    Task<bool> SetEntryTagsAsync(int entryId, List<string> tagNames);
    
    #endregion
    
    #region Statistics
    
    // Gets total number of journal entries
    Task<int> GetEntryCountAsync();
    
    // Gets total word count across all entries
    Task<int> GetTotalWordCountAsync();
    
    // Gets entries grouped by month
    Task<Dictionary<string, int>> GetEntriesByMonthAsync();
    
    // ===== NEW METHODS FOR MAIN DASHBOARD =====
    
    // Gets total number of entries (alias for GetEntryCountAsync)
    Task<int> GetTotalEntriesCountAsync();
    
    // Gets count of entries created this month
    Task<int> GetEntriesCountThisMonthAsync();
    
    // Gets average word count per entry
    Task<int> GetAverageWordCountAsync();
    
    // Gets the most frequently used primary mood
    Task<string> GetMostFrequentMoodAsync();
    
    // Gets recent entries (ordered by created date, newest first)
    Task<List<JournalEntry>> GetRecentEntriesAsync(int count);
    
    #endregion
}