using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Haihv.Vbdlis.Tools.Desktop.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Haihv.Vbdlis.Tools.Desktop.Services.Data;

/// <summary>
/// Service quản lý lịch sử tìm kiếm sử dụng EF Core
/// </summary>
public class SearchHistoryService(IDatabaseService databaseService)
{
    private const int MaxHistoryItems = 20;
    private readonly ILogger _logger = Log.ForContext<SearchHistoryService>();
    private readonly IDatabaseService _databaseService = databaseService;
    private bool _initialized;

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await _databaseService.InitializeDatabaseAsync();
        var dbContext = _databaseService.GetDbContext();
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS SearchHistoryEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SearchType TEXT NOT NULL,
                SearchQuery TEXT NOT NULL,
                ResultCount INTEGER NOT NULL DEFAULT 0,
                SearchItemCount INTEGER NOT NULL DEFAULT 0,
                Title TEXT NULL,
                SearchedAt TEXT NOT NULL
            );
            """);
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE UNIQUE INDEX IF NOT EXISTS IX_SearchHistoryEntries_SearchType_SearchQuery
            ON SearchHistoryEntries (SearchType, SearchQuery);
            """);
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS IX_SearchHistoryEntries_SearchType_SearchedAt
            ON SearchHistoryEntries (SearchType, SearchedAt);
            """);
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                "ALTER TABLE SearchHistoryEntries ADD COLUMN SearchItemCount INTEGER NOT NULL DEFAULT 0;");
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, "SearchItemCount column already exists or cannot be added.");
        }

        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                "ALTER TABLE SearchHistoryEntries ADD COLUMN Title TEXT NULL;");
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, "Title column already exists or cannot be added.");
        }

        _initialized = true;
    }

    public async Task<List<SearchHistoryEntry>> GetHistoryAsync(string searchType, int limit = MaxHistoryItems)
    {
        await InitializeAsync();
        var dbContext = _databaseService.GetDbContext();

        return await dbContext.SearchHistoryEntries
            .Where(x => x.SearchType == searchType)
            .OrderByDescending(x => x.SearchedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task UpsertAsync(
        string searchType,
        string searchQuery,
        int searchItemCount,
        int resultCount,
        DateTime searchedAt)
    {
        await InitializeAsync();
        var trimmedQuery = searchQuery.Trim();
        if (string.IsNullOrWhiteSpace(trimmedQuery))
        {
            return;
        }

        try
        {
            var dbContext = _databaseService.GetDbContext();
            var existing = await dbContext.SearchHistoryEntries
                .FirstOrDefaultAsync(x => x.SearchType == searchType && x.SearchQuery == trimmedQuery);

            if (existing != null)
            {
                existing.SearchItemCount = searchItemCount;
                existing.ResultCount = resultCount;
                existing.SearchedAt = searchedAt;
                dbContext.SearchHistoryEntries.Update(existing);
            }
            else
            {
                await dbContext.SearchHistoryEntries.AddAsync(new SearchHistoryEntry
                {
                    SearchType = searchType,
                    SearchQuery = trimmedQuery,
                    SearchItemCount = searchItemCount,
                    ResultCount = resultCount,
                    SearchedAt = searchedAt
                });
            }

            await dbContext.SaveChangesAsync();

            var extraItems = await dbContext.SearchHistoryEntries
                .Where(x => x.SearchType == searchType)
                .OrderByDescending(x => x.SearchedAt)
                .Skip(MaxHistoryItems)
                .ToListAsync();

            if (extraItems.Count > 0)
            {
                dbContext.SearchHistoryEntries.RemoveRange(extraItems);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to upsert search history for type {SearchType}", searchType);
        }
    }

    public async Task UpdateTitleAsync(int id, string? title)
    {
        await InitializeAsync();
        try
        {
            var dbContext = _databaseService.GetDbContext();
            var entry = await dbContext.SearchHistoryEntries.FirstOrDefaultAsync(x => x.Id == id);
            if (entry == null)
            {
                return;
            }

            entry.Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
            dbContext.SearchHistoryEntries.Update(entry);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update search history title for Id {Id}", id);
        }
    }
}