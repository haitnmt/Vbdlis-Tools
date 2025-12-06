using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haihv.Tools.Hsq.Entities;

/// <summary>
/// Entity cho bảng DvhcCapXa trong database
/// Lưu trữ thông tin Phường/Xã từ VBDLIS để cache
/// </summary>
[Table("DvhcCapXa")]
[Index(nameof(CapTinhId), nameof(CapHuyenId))]
public class DvhcCapXa
{
    /// <summary>
    /// Mã Đơn vị hành chính cấp xã
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Mã Đơn vị hành chính cấp tỉnh
    /// </summary>
    public int CapTinhId { get; set; }

    /// <summary>
    /// Mã Đơn vị hành chính cấp huyện
    /// </summary>
    public int CapHuyenId { get; set; }

    /// <summary>
    /// Tên Đơn vị hành chính cấp xã
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Cờ chỉ định đây là Đơn vị hành chính cấp xã 2 cấp
    /// </summary>
    public bool Dvhc2Cap { get; set; } = true;

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}