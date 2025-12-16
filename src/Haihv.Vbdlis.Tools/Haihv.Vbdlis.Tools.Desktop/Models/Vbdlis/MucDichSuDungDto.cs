using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// DTO đầy đủ cho Mục đích sử dụng
public class MucDichSuDungFullDto
{
    [JsonPropertyName("mucDichSuDungId")] public long MucDichSuDungId { get; set; }
    [JsonPropertyName("thuaDatId")] public long? ThuaDatId { get; set; }
    [JsonPropertyName("soThuTu")] public int? SoThuTu { get; set; }
    [JsonPropertyName("loaiMucDichSuDungId")] public string? LoaiMucDichSuDungId { get; set; }
    [JsonPropertyName("loaiMucDichSuDungQuyHoachId")] public string? LoaiMucDichSuDungQuyHoachId { get; set; }
    [JsonPropertyName("mucDichSuDungChiTiet")] public string? MucDichSuDungChiTiet { get; set; }
    [JsonPropertyName("loaiMucDichSuDungPhuId")] public string? LoaiMucDichSuDungPhuId { get; set; }
    [JsonPropertyName("dienTich")] public double? DienTich { get; set; }
    [JsonPropertyName("ghiChu")] public string? GhiChu { get; set; }
    [JsonPropertyName("ngayHinhThanh")] public string? NgayHinhThanh { get; set; }
    [JsonPropertyName("thoiHanSuDung")] public string? ThoiHanSuDung { get; set; }
    [JsonPropertyName("ngaySuDung")] public string? NgaySuDung { get; set; }
    [JsonPropertyName("versionThuaDat")] public int? VersionThuaDat { get; set; }
    [JsonPropertyName("isUpdate")] public bool? IsUpdate { get; set; }
    [JsonPropertyName("thuaDatBuildId")] public long? ThuaDatBuildId { get; set; }
    [JsonPropertyName("ListNguonGocSuDungDat")] public List<NguonGocSuDungDatFullDto>? ListNguonGocSuDungDat { get; set; }
    [JsonPropertyName("LoaiMucDichSuDung")] public LoaiMucDichSuDungFullDto? LoaiMucDichSuDung { get; set; }
    [JsonPropertyName("LoaiMucDichSuDungQuyHoach")] public object? LoaiMucDichSuDungQuyHoach { get; set; }
    [JsonPropertyName("LoaiMucDichSuDungPhu")] public object? LoaiMucDichSuDungPhu { get; set; }

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


// DTO cho Mục đích sử dụng
public class MucDichSuDungDto
{
    [JsonPropertyName("mucDichSuDungId")] public long MucDichSuDungId { get; set; }
    [JsonPropertyName("loaiMucDichSuDungId")] public string? LoaiMucDichSuDungId { get; set; }
    [JsonPropertyName("dienTich")] public double? DienTich { get; set; }
    [JsonPropertyName("LoaiMucDichSuDung")] public LoaiMucDichSuDungDto? LoaiMucDichSuDung { get; set; }
    [JsonPropertyName("ListNguonGocSuDungDat")] public List<NguonGocSuDungDatDto>? ListNguonGocSuDungDat { get; set; }
}

// DTO cho Loại mục đích sử dụng
public class LoaiMucDichSuDungDto
{
    [JsonPropertyName("loaiMucDichSuDungId")] public string? LoaiMucDichSuDungId { get; set; }
    [JsonPropertyName("tenLoaiMucDichSuDung")] public string? TenLoaiMucDichSuDung { get; set; }
}


// DTO đầy đủ cho Loại mục đích sử dụng
public class LoaiMucDichSuDungFullDto
{
    [JsonPropertyName("loaiMucDichSuDungId")] public string? LoaiMucDichSuDungId { get; set; }
    [JsonPropertyName("kyHieuLoaiMucDichSuDung")] public string? KyHieuLoaiMucDichSuDung { get; set; }
    [JsonPropertyName("tenLoaiMucDichSuDung")] public string? TenLoaiMucDichSuDung { get; set; }
    [JsonPropertyName("moTaLoaiMucDichSuDung")] public string? MoTaLoaiMucDichSuDung { get; set; }
    [JsonPropertyName("trangThai")] public bool? TrangThai { get; set; }
}