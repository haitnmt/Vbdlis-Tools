using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// DTO đầy đủ cho Nguồn gốc sử dụng đất
public class NguonGocSuDungDatFullDto
{
    [JsonPropertyName("nguonGocSuDungDatId")] public long NguonGocSuDungDatId { get; set; }
    [JsonPropertyName("mucDichSuDungId")] public long? MucDichSuDungId { get; set; }
    [JsonPropertyName("thuaDatId")] public long? ThuaDatId { get; set; }
    [JsonPropertyName("dienTich")] public double? DienTich { get; set; }
    [JsonPropertyName("loaiNguonGocSuDungDatId")] public int? LoaiNguonGocSuDungDatId { get; set; }
    [JsonPropertyName("loaiNguonGocChuyenQuyenId")] public int? LoaiNguonGocChuyenQuyenId { get; set; }
    [JsonPropertyName("chiTiet")] public string? ChiTiet { get; set; }
    [JsonPropertyName("versionThuaDat")] public int? VersionThuaDat { get; set; }
    [JsonPropertyName("LoaiNguonGocSuDungDat")] public LoaiNguonGocSuDungDatFullDto? LoaiNguonGocSuDungDat { get; set; }
    [JsonPropertyName("LoaiNguonGocChuyenQuyen")] public LoaiNguonGocChuyenQuyenDto? LoaiNguonGocChuyenQuyen { get; set; }
    [JsonPropertyName("soThuTuMucDich")] public int? SoThuTuMucDich { get; set; }

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


// DTO cho Nguồn gốc sử dụng đất
public class NguonGocSuDungDatDto
{
    [JsonPropertyName("nguonGocSuDungDatId")] public long NguonGocSuDungDatId { get; set; }
    [JsonPropertyName("dienTich")] public double? DienTich { get; set; }
    [JsonPropertyName("LoaiNguonGocSuDungDat")] public LoaiNguonGocSuDungDatDto? LoaiNguonGocSuDungDat { get; set; }
    [JsonPropertyName("LoaiNguonGocChuyenQuyen")] public LoaiNguonGocChuyenQuyenDto? LoaiNguonGocChuyenQuyen { get; set; }
}

// DTO cho Loại nguồn gốc sử dụng đất
public class LoaiNguonGocSuDungDatDto
{
    [JsonPropertyName("loaiNguonGocSuDungDatId")] public int LoaiNguonGocSuDungDatId { get; set; }
    [JsonPropertyName("tenLoaiNguonGocInGiay")] public string? TenLoaiNguonGocInGiay { get; set; }
}

// DTO cho Loại nguồn gốc chuyển quyền
public class LoaiNguonGocChuyenQuyenDto
{
    [JsonPropertyName("loaiNguonGocChuyenQuyenId")] public int LoaiNguonGocChuyenQuyenId { get; set; }
    [JsonPropertyName("tenNguonGocChuyenQuyen")] public string? TenNguonGocChuyenQuyen { get; set; }
}
// DTO đầy đủ cho Loại nguồn gốc sử dụng đất
public class LoaiNguonGocSuDungDatFullDto
{
    [JsonPropertyName("loaiNguonGocSuDungDatId")] public int LoaiNguonGocSuDungDatId { get; set; }
    [JsonPropertyName("maLoaiNguonGocSuDungDat")] public string? MaLoaiNguonGocSuDungDat { get; set; }
    [JsonPropertyName("tenLoaiNguonGocInGiay")] public string? TenLoaiNguonGocInGiay { get; set; }
    [JsonPropertyName("tenLoaiNguonGocSoDiaChinh")] public string? TenLoaiNguonGocSoDiaChinh { get; set; }
    [JsonPropertyName("moTa")] public string? MoTa { get; set; }
    [JsonPropertyName("trangThai")] public bool? TrangThai { get; set; }
}