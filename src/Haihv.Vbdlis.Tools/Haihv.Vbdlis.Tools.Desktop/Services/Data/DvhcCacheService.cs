using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Haihv.Vbdlis.Tools.Desktop.Entities;
using Serilog;

namespace Haihv.Vbdlis.Tools.Desktop.Services.Data;

/// <summary>
/// Service quản lý cache Đơn vị hành chính (ĐVHC) sử dụng EF Core
/// </summary>
public class DvhcCacheService
{
    private readonly ILogger _logger = Log.ForContext<DvhcCacheService>();
    private readonly IDatabaseService _databaseService;

    public DvhcCacheService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    #region DvhcCapHuyen

    /// <summary>
    /// Lưu danh sách huyện vào database
    /// </summary>
    public async Task SaveCapHuyenListAsync(int tinhId, List<DvhcCapHuyen> capHuyenList)
    {
        try
        {
            var dbContext = _databaseService.GetDbContext();

            // Xóa dữ liệu cũ của tỉnh này
            var existingRecords = await dbContext.DvhcCapHuyen
                .Where(x => x.CapTinhId == tinhId)
                .ToListAsync();

            if (existingRecords.Any())
            {
                dbContext.DvhcCapHuyen.RemoveRange(existingRecords);
            }

            // Thêm dữ liệu mới
            await dbContext.DvhcCapHuyen.AddRangeAsync(capHuyenList);
            await dbContext.SaveChangesAsync();

            _logger.Information("Saved {Count} districts for province {TinhId}", capHuyenList.Count, tinhId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to save districts for province {TinhId}", tinhId);
            throw;
        }
    }

    /// <summary>
    /// Lấy danh sách huyện từ database
    /// </summary>
    public async Task<List<DvhcCapHuyen>> GetCapHuyenListAsync(int tinhId)
    {
        try
        {
            var dbContext = _databaseService.GetDbContext();
            var result = await dbContext.DvhcCapHuyen
                .Where(x => x.CapTinhId == tinhId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            _logger.Debug("Retrieved {Count} districts for province {TinhId}", result.Count, tinhId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to get districts for province {TinhId}", tinhId);
            return new List<DvhcCapHuyen>();
        }
    }

    #endregion

    #region DvhcCapXa

    /// <summary>
    /// Lưu danh sách xã vào database
    /// </summary>
    public async Task SaveCapXaListAsync(int tinhId, List<DvhcCapXa> capXaList)
    {
        try
        {
            var dbContext = _databaseService.GetDbContext();

            // Xóa dữ liệu cũ của tỉnh này
            var existingRecords = await dbContext.DvhcCapXa
                .Where(x => x.CapTinhId == tinhId)
                .ToListAsync();

            if (existingRecords.Any())
            {
                dbContext.DvhcCapXa.RemoveRange(existingRecords);
            }

            // Thêm dữ liệu mới
            await dbContext.DvhcCapXa.AddRangeAsync(capXaList);
            await dbContext.SaveChangesAsync();

            _logger.Information("Saved {Count} wards for province {TinhId}", capXaList.Count, tinhId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to save wards for province {TinhId}", tinhId);
            throw;
        }
    }

    /// <summary>
    /// Lấy danh sách xã theo tỉnh từ database
    /// </summary>
    public async Task<List<DvhcCapXa>> GetCapXaListByTinhAsync(int tinhId)
    {
        try
        {
            var dbContext = _databaseService.GetDbContext();
            var result = await dbContext.DvhcCapXa
                .Where(x => x.CapTinhId == tinhId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            _logger.Debug("Retrieved {Count} wards for province {TinhId}", result.Count, tinhId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to get wards for province {TinhId}", tinhId);
            return new List<DvhcCapXa>();
        }
    }

    /// <summary>
    /// Lấy danh sách xã theo huyện từ database
    /// </summary>
    public async Task<List<DvhcCapXa>> GetCapXaListByHuyenAsync(int tinhId, int huyenId)
    {
        try
        {
            var dbContext = _databaseService.GetDbContext();
            var result = await dbContext.DvhcCapXa
                .Where(x => x.CapTinhId == tinhId && x.CapHuyenId == huyenId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            _logger.Debug("Retrieved {Count} wards for district {HuyenId}", result.Count, huyenId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to get wards for district {HuyenId}", huyenId);
            return new List<DvhcCapXa>();
        }
    }

    #endregion
}
