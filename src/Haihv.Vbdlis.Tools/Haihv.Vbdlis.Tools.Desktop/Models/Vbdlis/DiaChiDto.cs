using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// DTO đầy đủ cho Địa chỉ
public class DiaChiFullDto
{
    [JsonPropertyName("diaChiId")] public long DiaChiId { get; set; }
    [JsonPropertyName("typeItem")] public int? TypeItem { get; set; }
    [JsonPropertyName("itemId")] public long? ItemId { get; set; }
    [JsonPropertyName("tinhId")] public int? TinhId { get; set; }
    [JsonPropertyName("huyenId")] public int? HuyenId { get; set; }
    [JsonPropertyName("xaId")] public int? XaId { get; set; }
    [JsonPropertyName("diaChiChiTiet")] public string? DiaChiChiTiet { get; set; }
    [JsonPropertyName("soNha")] public string? SoNha { get; set; }
    [JsonPropertyName("ngoPho")] public string? NgoPho { get; set; }
    [JsonPropertyName("duongId")] public int? DuongId { get; set; }
    [JsonPropertyName("tenDuong")] public string? TenDuong { get; set; }
    [JsonPropertyName("toDanPhoId")] public int? ToDanPhoId { get; set; }
    [JsonPropertyName("tenToDanPho")] public string? TenToDanPho { get; set; }
    [JsonPropertyName("laDiaChiCu")] public bool? LaDiaChiCu { get; set; }
    [JsonPropertyName("laDiaChiChinh")] public bool LaDiaChiChinh { get; set; }
    [JsonPropertyName("trangThai")] public bool? TrangThai { get; set; }
    [JsonPropertyName("versionItem")] public int? VersionItem { get; set; }
    [JsonPropertyName("maDiaChiSo")] public string? MaDiaChiSo { get; set; }
    [JsonPropertyName("Tinh")] public object? Tinh { get; set; }
    [JsonPropertyName("Huyen")] public object? Huyen { get; set; }
    [JsonPropertyName("Xa")] public object? Xa { get; set; }
    [JsonPropertyName("Duong")] public object? Duong { get; set; }
    [JsonPropertyName("ToDanPho")] public object? ToDanPho { get; set; }

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
