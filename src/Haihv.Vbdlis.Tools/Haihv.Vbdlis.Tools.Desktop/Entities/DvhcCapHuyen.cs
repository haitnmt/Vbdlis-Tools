using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Haihv.Tools.Hsq.Entities;

/// <summary>
/// Entity cho bảng DvhcCapHuyen trong database
/// Lưu trữ thông tin Quận/Huyện từ VBDLIS để cache
/// </summary>
[Table("DvhcCapHuyen")]
[Index(nameof(CapTinhId))]
public class DvhcCapHuyen
{
    /// <summary>
    /// ID của Quận/Huyện từ VBDLIS
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID của Tỉnh/Thành phố cấp trên
    /// </summary>
    public int CapTinhId { get; set; }

    /// <summary>
    /// Tên Quận/Huyện (tên gốc từ web)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}