using System;
using System.Collections.Generic;
using System.Linq;

namespace Haihv.Vbdlis.Tools.Desktop.Models;

public class ThuaDat(int xaId, string soToBanDo, string soThuaDat, double? dienTich, string mucDichSuDung, string diaChi, List<MucDichSuDungInfo>? listMucDichSuDung)
{
    public int XaId { get; set; } = xaId;
    public string SoToBanDo { get; } = soToBanDo;
    public string SoThuaDat { get; } = soThuaDat;
    public double? DienTich { get; } = dienTich;
    public string MucDichSuDung { get; } = mucDichSuDung;

    public string DiaChi
    { get; } = diaChi;
    public List<MucDichSuDungInfo> ListMucDichSuDung { get; } = listMucDichSuDung ?? [];

    public string DienTichFormatted => DienTich.HasValue && DienTich.Value > 0
        ? $"{DienTich.Value} m²"
        : "";

    public bool HasDienTich => DienTich.HasValue && DienTich.Value > 0;

    public string MucDichSuDungFormatted
    {
        get
        {
            if (ListMucDichSuDung == null || ListMucDichSuDung.Count == 0)
                return MucDichSuDung;

            var parts = ListMucDichSuDung
                .Where(m => !string.IsNullOrWhiteSpace(m.LoaiMucDichSuDungId) && m.DienTich > 0)
                .Select(m => $"{m.LoaiMucDichSuDungId} ({m.DienTich} m²)");

            return string.Join(", ", parts);
        }
    }

    public bool HasMucDichSuDung => ListMucDichSuDung != null && ListMucDichSuDung.Count > 0;

    public string NguonGocSuDungDatFormatted
    {
        get
        {
            if (ListMucDichSuDung == null || ListMucDichSuDung.Count == 0)
                return "";

            var allNguonGoc = ListMucDichSuDung
                .SelectMany(m => m.ListNguonGocSuDungDat)
                .Where(n => !string.IsNullOrWhiteSpace(n.TenLoaiNguonGocInGiay) && n.DienTich > 0)
                .Select(n =>
                {
                    var prefix = string.IsNullOrWhiteSpace(n.TenNguonGocChuyenQuyen?.Trim())
                        ? ""
                        : $"{n.TenNguonGocChuyenQuyen.Trim()} ";
                    return $"{prefix}{n.TenLoaiNguonGocInGiay} ({n.DienTich} m²)";
                });

            return string.Join(", ", allNguonGoc);
        }
    }

    public bool HasNguonGocSuDungDat =>
        ListMucDichSuDung != null &&
        ListMucDichSuDung.Any(m => m.ListNguonGocSuDungDat != null && m.ListNguonGocSuDungDat.Count > 0);

    public string ThuaDatCompact
    {
        get
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(SoToBanDo))
                parts.Add($"Tờ bản đồ số: {SoToBanDo}");

            if (!string.IsNullOrWhiteSpace(SoThuaDat))
                parts.Add($"Thửa đất số: {SoThuaDat}");

            if (HasDienTich)
                parts.Add($"Diện tích: {DienTichFormatted}");

            if (HasMucDichSuDung)
                parts.Add($"Mục đích sử dụng: {MucDichSuDungFormatted}");
            else if (!string.IsNullOrWhiteSpace(MucDichSuDung))
                parts.Add($"Mục đích sử dụng: {MucDichSuDung}");

            if (HasNguonGocSuDungDat)
                parts.Add($"Nguồn gốc sử dụng đất: {NguonGocSuDungDatFormatted}");

            if (!string.IsNullOrWhiteSpace(DiaChi))
                parts.Add($"Địa chỉ: {DiaChi}");

            return string.Join(Environment.NewLine, parts);
        }
    }
}

public class MucDichSuDungInfo(string loaiMucDichSuDungId, double dienTich, List<NguonGocSuDungDatInfo>? listNguonGocSuDungDat = null)
{
    public string LoaiMucDichSuDungId { get; } = loaiMucDichSuDungId;
    public double DienTich { get; } = dienTich;
    public List<NguonGocSuDungDatInfo> ListNguonGocSuDungDat { get; } = listNguonGocSuDungDat ?? [];
}

public class NguonGocSuDungDatInfo(string tenNguonGocChuyenQuyen, string tenLoaiNguonGocInGiay, double dienTich)
{
    public string TenNguonGocChuyenQuyen { get; } = tenNguonGocChuyenQuyen;
    public string TenLoaiNguonGocInGiay { get; } = tenLoaiNguonGocInGiay;
    public double DienTich { get; } = dienTich;
}