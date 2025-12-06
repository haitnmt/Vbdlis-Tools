using System.Threading.Tasks;
using Haihv.Vbdlis.Tools.Desktop.Data;

namespace Haihv.Vbdlis.Tools.Desktop.Services.Data;

/// <summary>
/// Interface cho Database Service sử dụng Entity Framework Core
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Lấy DbContext instance
    /// </summary>
    VbdlisDbContext GetDbContext();

    /// <summary>
    /// Khởi tạo database và chạy migrations
    /// </summary>
    Task InitializeDatabaseAsync();

    /// <summary>
    /// Đóng và dispose database connection
    /// </summary>
    void CloseDatabase();

    /// <summary>
    /// Lấy đường dẫn đến file database
    /// </summary>
    string GetDatabasePath();
}
