using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// DTO đầy đủ cho Hộ gia đình
public class HoGiaDinhFullDto
{
    [JsonPropertyName("hoGiaDinhId")] public long HoGiaDinhId { get; set; }
    [JsonPropertyName("chuHoId")] public long? ChuHoId { get; set; }
    [JsonPropertyName("voChongChuHoId")] public long? VoChongChuHoId { get; set; }
    [JsonPropertyName("soSoHoKhau")] public string? SoSoHoKhau { get; set; }
    [JsonPropertyName("hoSoHoKhauSo")] public string? HoSoHoKhauSo { get; set; }
    [JsonPropertyName("xaId")] public int? XaId { get; set; }
    [JsonPropertyName("diaChi")] public string? DiaChi { get; set; }
    [JsonPropertyName("version")] public int? Version { get; set; }
    [JsonPropertyName("versionChuHo")] public int? VersionChuHo { get; set; }
    [JsonPropertyName("versionVoChongChuHo")] public int? VersionVoChongChuHo { get; set; }
    [JsonPropertyName("isLastest")] public bool? IsLastest { get; set; }
    [JsonPropertyName("isNew")] public bool? IsNew { get; set; }
    [JsonPropertyName("isChange")] public bool? IsChange { get; set; }
    [JsonPropertyName("VoChong")] public CaNhanDto? VoChong { get; set; }
    [JsonPropertyName("ChuHo")] public CaNhanDto? ChuHo { get; set; }
    [JsonPropertyName("ListDiaChi")] public List<DiaChiFullDto>? ListDiaChi { get; set; }
    [JsonPropertyName("ListThanhVienHoGiaDinh")] public List<CaNhanDto>? ListThanhVienHoGiaDinh { get; set; }

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
