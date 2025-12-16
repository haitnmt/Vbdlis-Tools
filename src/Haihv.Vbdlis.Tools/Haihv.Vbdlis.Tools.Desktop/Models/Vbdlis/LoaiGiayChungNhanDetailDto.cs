using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// Chi tiết Loại Giấy Chứng Nhận (khớp với mẫu JSON)
public class LoaiGiayChungNhanDetailDto
{
    [JsonPropertyName("loaiGiayChungNhanId")]
    public int? LoaiGiayChungNhanId { get; set; }

    [JsonPropertyName("maLoai")] public string? MaLoai { get; set; }
    [JsonPropertyName("tenLoai")] public string? TenLoai { get; set; }
    [JsonPropertyName("laGiayDat")] public bool? LaGiayDat { get; set; }
    [JsonPropertyName("sapXep")] public int? SapXep { get; set; }

    [JsonPropertyName("trangThai")] public bool? TrangThai { get; set; }

    // Metadata chung
    [JsonPropertyName("Id")] public string? Id { get; set; }
    [JsonPropertyName("Title")] public string? Title { get; set; }
    [JsonPropertyName("Description")] public string? Description { get; set; }
    [JsonPropertyName("Name")] public string? Name { get; set; }
    [JsonPropertyName("Path")] public string? Path { get; set; }
    [JsonPropertyName("ParentPath")] public string? ParentPath { get; set; }
    [JsonPropertyName("Layer")] public string? Layer { get; set; }
    [JsonPropertyName("InId")] public string? InId { get; set; }
    [JsonPropertyName("OutId")] public string? OutId { get; set; }
}
