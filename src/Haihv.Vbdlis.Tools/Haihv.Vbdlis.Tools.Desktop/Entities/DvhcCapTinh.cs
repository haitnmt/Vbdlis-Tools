using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haihv.Vbdlis.Tools.Desktop.Entities;

/// <summary>
/// Entity cho bảng DvhcCapTinh trong database
/// Lưu trữ thông tin Tỉnh/Thành phố từ VBDLIS để cache
/// </summary>
[Table("DvhcCapTinh")]
public class DvhcCapTinh
{
    /// <summary>
    /// Mã Đơn vị hành chính cấp tỉnh
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Tên Đơn vị hành chính cấp tỉnh
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
