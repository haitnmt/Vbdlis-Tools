using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

public class TaiSanDto
{
    [JsonPropertyName("soThuTuThua")] public int? SoThuTuThua { get; set; }
    [JsonPropertyName("soHieuToBanDo")] public int? SoHieuToBanDo { get; set; }
    [JsonPropertyName("diaChi")] public string? DiaChi { get; set; }
    [JsonPropertyName("soHieuCanHo")] public string? SoHieuCanHo { get; set; }
}


// DTO cho Loại nhà riêng lẻ
public class LoaiNhaRiengLeDto
{
    [JsonPropertyName("loaiNhaRiengLeId")] public int LoaiNhaRiengLeId { get; set; }
    [JsonPropertyName("code")] public string? Code { get; set; }
    [JsonPropertyName("detail")] public string? Detail { get; set; }
}

// DTO cho Căn hộ
public class CanHoDto
{
    [JsonPropertyName("canHoId")] public long CanHoId { get; set; }
    [JsonPropertyName("tenCanHo")] public string? TenCanHo { get; set; }
    [JsonPropertyName("soHieuCanHo")] public string? SoHieuCanHo { get; set; }
    [JsonPropertyName("tangSo")] public string? TangSo { get; set; }
    [JsonPropertyName("dienTichSan")] public double? DienTichSan { get; set; }
    [JsonPropertyName("dienTichSuDung")] public double? DienTichSuDung { get; set; }
    [JsonPropertyName("diaChiCanHo")] public string? DiaChiCanHo { get; set; }
    [JsonPropertyName("ListLienKetTaiSanThuaDat")] public List<LienKetTaiSanThuaDatDto>? ListLienKetTaiSanThuaDat { get; set; }
    [JsonPropertyName("nhaChungCu")] public NhaChungCuDto? NhaChungCu { get; set; }
}

// DTO cho Nhà chung cư
public class NhaChungCuDto
{
    [JsonPropertyName("nhaChungCuId")] public long NhaChungCuId { get; set; }
    [JsonPropertyName("tenChungCu")] public string? TenChungCu { get; set; }
    [JsonPropertyName("ListThuaLienKet")] public List<LienKetTaiSanThuaDatDto>? ListThuaLienKet { get; set; }
}

// DTO cho Liên kết tài sản - thửa đất
public class LienKetTaiSanThuaDatDto
{
    [JsonPropertyName("lienKetTaiSanThuaDatId")] public long LienKetTaiSanThuaDatId { get; set; }
    [JsonPropertyName("thuaDatId")] public long? ThuaDatId { get; set; }
    [JsonPropertyName("thuaDat")] public ThuaDatFullDto? ThuaDat { get; set; }
}


// DTO cho Địa chỉ
public class DiaChiDto
{
    [JsonPropertyName("diaChiId")] public long DiaChiId { get; set; }
    [JsonPropertyName("diaChiChiTiet")] public string? DiaChiChiTiet { get; set; }
    [JsonPropertyName("laDiaChiChinh")] public bool LaDiaChiChinh { get; set; }
}

// DTO cho Nhà riêng lẻ
public class NhaRiengLeDto
{
    [JsonPropertyName("nhaRiengLeId")] public long NhaRiengLeId { get; set; }
    [JsonPropertyName("loaiNhaRiengLeId")] public int? LoaiNhaRiengLeId { get; set; }
    [JsonPropertyName("dienTichXayDung")] public double? DienTichXayDung { get; set; }
    [JsonPropertyName("dienTichSan")] public double? DienTichSan { get; set; }
    [JsonPropertyName("dienTichSuDung")] public double? DienTichSuDung { get; set; }
    [JsonPropertyName("soTang")] public string? SoTang { get; set; }
    [JsonPropertyName("diaChi")] public string? DiaChi { get; set; }
    [JsonPropertyName("ListDiaChi")] public List<DiaChiDto>? ListDiaChi { get; set; }
    [JsonPropertyName("ListLienKetTaiSanThuaDat")] public List<LienKetTaiSanThuaDatDto>? ListLienKetTaiSanThuaDat { get; set; }
    [JsonPropertyName("LoaiNhaRiengLe")] public LoaiNhaRiengLeDto? LoaiNhaRiengLe { get; set; }
}