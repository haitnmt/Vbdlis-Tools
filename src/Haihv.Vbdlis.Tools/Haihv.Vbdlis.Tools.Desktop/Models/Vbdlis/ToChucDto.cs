using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// DTO cho Tổ chức
public class ToChucDto
{
    [JsonPropertyName("toChucId")] public long ToChucId { get; set; }
    [JsonPropertyName("maSoDinhDanh")] public string? MaSoDinhDanh { get; set; }
    [JsonPropertyName("tenToChuc")] public string? TenToChuc { get; set; }
    [JsonPropertyName("tenVietTat")] public string? TenVietTat { get; set; }
    [JsonPropertyName("tenToChucTA")] public string? TenToChucTA { get; set; }
    [JsonPropertyName("nguoiDaiDienId")] public long? NguoiDaiDienId { get; set; }
    [JsonPropertyName("loaiQuyetDinhThanhLap")] public string? LoaiQuyetDinhThanhLap { get; set; }
    [JsonPropertyName("soQuyetDinh")] public string? SoQuyetDinh { get; set; }
    [JsonPropertyName("ngayQuyetDinh")] public string? NgayQuyetDinh { get; set; }
    [JsonPropertyName("maDoanhNghiep")] public string? MaDoanhNghiep { get; set; }
    [JsonPropertyName("maSoThue")] public string? MaSoThue { get; set; }
    [JsonPropertyName("loaiToChucId")] public int? LoaiToChucId { get; set; }
    [JsonPropertyName("xaId")] public int? XaId { get; set; }
    [JsonPropertyName("diaChi")] public string? DiaChi { get; set; }
    [JsonPropertyName("version")] public int? Version { get; set; }
    [JsonPropertyName("versionNguoiDaiDien")] public int? VersionNguoiDaiDien { get; set; }
    [JsonPropertyName("isLastest")] public bool? IsLastest { get; set; }
    [JsonPropertyName("laDoiTuongQuanLyDat")] public bool? LaDoiTuongQuanLyDat { get; set; }
    [JsonPropertyName("isNew")] public bool? IsNew { get; set; }
    [JsonPropertyName("isChange")] public bool? IsChange { get; set; }
    [JsonPropertyName("NguoiDaiDien")] public object? NguoiDaiDien { get; set; }
    [JsonPropertyName("ListDiaChi")] public List<DiaChiFullDto>? ListDiaChi { get; set; }
    [JsonPropertyName("ListGiayToBoSung")] public List<GiayToToChucDto>? ListGiayToBoSung { get; set; }
    [JsonPropertyName("LoaiToChuc")] public object? LoaiToChuc { get; set; }

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


// DTO cho Giấy tờ tổ chức
public class GiayToToChucDto
{
    [JsonPropertyName("giayToToChucId")] public long GiayToToChucId { get; set; }
    [JsonPropertyName("toChucId")] public long? ToChucId { get; set; }
    [JsonPropertyName("loaiGiayToToChucId")] public int? LoaiGiayToToChucId { get; set; }
    [JsonPropertyName("soGiayTo")] public string? SoGiayTo { get; set; }
    [JsonPropertyName("ngayCap")] public string? NgayCap { get; set; }
    [JsonPropertyName("noiCap")] public string? NoiCap { get; set; }
    [JsonPropertyName("ngayHetHan")] public string? NgayHetHan { get; set; }
    [JsonPropertyName("ghiChu")] public string? GhiChu { get; set; }
    [JsonPropertyName("versionToChuc")] public int? VersionToChuc { get; set; }
    [JsonPropertyName("hinhThucXacThuc")] public int? HinhThucXacThuc { get; set; }
    [JsonPropertyName("maDinhDanhDoanhNghiep")] public string? MaDinhDanhDoanhNghiep { get; set; }
    [JsonPropertyName("LoaiGiayToToChuc")] public object? LoaiGiayToToChuc { get; set; }

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