using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// DTO đầy đủ cho Vợ chồng
public class VoChongDto
{
    [JsonPropertyName("voChongId")] public long VoChongId { get; set; }
    [JsonPropertyName("voId")] public long? VoId { get; set; }
    [JsonPropertyName("chongId")] public long? ChongId { get; set; }
    [JsonPropertyName("soGiayChungNhanKetHon")] public string? SoGiayChungNhanKetHon { get; set; }
    [JsonPropertyName("quyenSoGiayChungNhanKetHon")] public string? QuyenSoGiayChungNhanKetHon { get; set; }
    [JsonPropertyName("thongTinDaiDien")] public string? ThongTinDaiDien { get; set; }
    [JsonPropertyName("xaId")] public int? XaId { get; set; }
    [JsonPropertyName("thoiDiemHinhThanh")] public string? ThoiDiemHinhThanh { get; set; }
    [JsonPropertyName("thoiDiemKetThuc")] public string? ThoiDiemKetThuc { get; set; }
    [JsonPropertyName("tinhTrangHonNhan")] public bool? TinhTrangHonNhan { get; set; }
    [JsonPropertyName("inVoTruoc")] public bool? InVoTruoc { get; set; }
    [JsonPropertyName("version")] public int? Version { get; set; }
    [JsonPropertyName("versionChong")] public int? VersionChong { get; set; }
    [JsonPropertyName("versionVo")] public int? VersionVo { get; set; }
    [JsonPropertyName("isLastest")] public bool? IsLastest { get; set; }
    [JsonPropertyName("isNew")] public bool? IsNew { get; set; }
    [JsonPropertyName("isChange")] public bool? IsChange { get; set; }
    [JsonPropertyName("Chong")] public CaNhanDto? Chong { get; set; }
    [JsonPropertyName("Vo")] public CaNhanDto? Vo { get; set; }

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
