using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// DTO đầy đủ cho Cá nhân (với ListGiayToTuyThan và ListDiaChi)
public class CaNhanDto
{
    [JsonPropertyName("caNhanId")] public long CaNhanId { get; set; }
    [JsonPropertyName("maSoDinhDanh")] public string? MaSoDinhDanh { get; set; }
    [JsonPropertyName("loaiDoiTuongId")] public int? LoaiDoiTuongId { get; set; }
    [JsonPropertyName("maSoThue")] public string? MaSoThue { get; set; }
    [JsonPropertyName("gioiTinh")] public bool? GioiTinh { get; set; }
    [JsonPropertyName("hoTen")] public string? HoTen { get; set; }
    [JsonPropertyName("ngaySinh")] public string? NgaySinh { get; set; }
    [JsonPropertyName("namSinh")] public int? NamSinh { get; set; }
    [JsonPropertyName("namMat")] public int? NamMat { get; set; }
    [JsonPropertyName("soDienThoai")] public string? SoDienThoai { get; set; }
    [JsonPropertyName("diaChiEmail")] public string? DiaChiEmail { get; set; }
    [JsonPropertyName("quocTichId1")] public int? QuocTichId1 { get; set; }
    [JsonPropertyName("quocTichId2")] public int? QuocTichId2 { get; set; }
    [JsonPropertyName("danTocId")] public int? DanTocId { get; set; }
    [JsonPropertyName("xaId")] public int? XaId { get; set; }
    [JsonPropertyName("diaChi")] public string? DiaChi { get; set; }
    [JsonPropertyName("version")] public int? Version { get; set; }
    [JsonPropertyName("isLastest")] public bool? IsLastest { get; set; }
    [JsonPropertyName("isNew")] public bool? IsNew { get; set; }
    [JsonPropertyName("isChange")] public bool? IsChange { get; set; }
    [JsonPropertyName("isCaNhan1")] public bool? IsCaNhan1 { get; set; }
    [JsonPropertyName("maChuSuDung")] public string? MaChuSuDung { get; set; }
    [JsonPropertyName("ListGiayToTuyThan")] public List<GiayToTuyThanDto>? ListGiayToTuyThan { get; set; }
    [JsonPropertyName("ListDiaChi")] public List<DiaChiFullDto>? ListDiaChi { get; set; }
    [JsonPropertyName("LoaiDoiTuong")] public object? LoaiDoiTuong { get; set; }
    [JsonPropertyName("QuocTich1")] public object? QuocTich1 { get; set; }
    [JsonPropertyName("QuocTich2")] public object? QuocTich2 { get; set; }
    [JsonPropertyName("DanToc")] public object? DanToc { get; set; }

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


// DTO cho Loại giấy tờ tùy thân
public class LoaiGiayToTuyThanDto
{
    [JsonPropertyName("loaiGiayToTuyThanId")] public int LoaiGiayToTuyThanId { get; set; }
    [JsonPropertyName("maLoaiGiayTo")] public string? MaLoaiGiayTo { get; set; }
    [JsonPropertyName("tenLoaiGiayTo")] public string? TenLoaiGiayTo { get; set; }
}

// DTO cho Giấy tờ tùy thân
public class GiayToTuyThanDto
{
    [JsonPropertyName("CreatedDate")] public string? CreatedDate { get; set; }
    [JsonPropertyName("ModifiedDate")] public string? ModifiedDate { get; set; }
    [JsonPropertyName("giayToTuyThanId")] public long GiayToTuyThanId { get; set; }
    [JsonPropertyName("caNhanId")] public long? CaNhanId { get; set; }
    [JsonPropertyName("laThongTinChinh")] public bool? LaThongTinChinh { get; set; }
    [JsonPropertyName("loaiGiayToTuyThanId")] public int? LoaiGiayToTuyThanId { get; set; }
    [JsonPropertyName("soGiayTo")] public string? SoGiayTo { get; set; }
    [JsonPropertyName("ngayCap")] public string? NgayCap { get; set; }
    [JsonPropertyName("noiCap")] public string? NoiCap { get; set; }
    [JsonPropertyName("ngayHetHan")] public string? NgayHetHan { get; set; }
    [JsonPropertyName("ghiChu")] public string? GhiChu { get; set; }
    [JsonPropertyName("daDinhDanh")] public bool? DaDinhDanh { get; set; }
    [JsonPropertyName("trangThaiXacThuc")] public int? TrangThaiXacThuc { get; set; }
    [JsonPropertyName("versionCaNhan")] public int? VersionCaNhan { get; set; }
    [JsonPropertyName("hinhThucXacThuc")] public int? HinhThucXacThuc { get; set; }
    [JsonPropertyName("maDinhDanhCaNhan")] public string? MaDinhDanhCaNhan { get; set; }
    [JsonPropertyName("groupKey")] public string? GroupKey { get; set; }
    [JsonPropertyName("LoaiGiayToTuyThan")] public LoaiGiayToTuyThanDto? LoaiGiayToTuyThan { get; set; }

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