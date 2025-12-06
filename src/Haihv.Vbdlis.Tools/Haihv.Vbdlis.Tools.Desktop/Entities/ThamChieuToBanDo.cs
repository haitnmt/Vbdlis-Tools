using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haihv.Vbdlis.Tools.Desktop.Entities;

/// <summary>
/// Entity lưu thông tin về tham chiếu tờ bản đồ (cũ - mới)
/// </summary>
[Table("ThamChieuToBanDo")]
[Index(nameof(TinhId), nameof(XaId))]
[Index(nameof(SoToBanDo))]
public class ThamChieuToBanDo
{
    /// <summary>
    /// Id của bản ghi 
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// Số tờ bản đồ mới
    /// </summary>
    public int SoToBanDo { get; set; } = 0;

    /// <summary>
    /// Số tờ bản đồ cũ
    /// </summary>
    [MaxLength(100)]
    public string SoToBanDoCu { get; set; } = string.Empty;

    /// <summary>
    /// Mã đơn vị hành chính cấp tỉnh
    /// </summary>
    public int TinhId { get; set; }

    /// <summary>
    /// Mã đơn vị hành chính cấp xã
    /// </summary>
    public int XaId { get; set; }

    /// <summary>
    /// Mã đơn vị hành chính cấp tỉnh cũ (trước sáp nhập)
    /// </summary>
    public int TinhCuId { get; set; }

    /// <summary>
    /// Mã đơn vị hành chính cấp xã cũ (trước sáp nhập)
    /// </summary>
    public int XaCuId { get; set; }

    /// <summary>
    /// Thời điểm tạo bản ghi
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm cập nhật bản ghi
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }
}