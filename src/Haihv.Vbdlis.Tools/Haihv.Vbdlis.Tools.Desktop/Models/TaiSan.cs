using System;
using System.Collections.Generic;

namespace Haihv.Vbdlis.Tools.Desktop.Models;

public class TaiSan(string tenTaiSan, double? dienTichXayDung, double? dienTichSuDung, string soTang, string diaChi)
{
    public string TenTaiSan { get; } = tenTaiSan;
    public double? DienTichXayDung { get; } = dienTichXayDung;
    public double? DienTichSuDung { get; } = dienTichSuDung;
    public string SoTang { get; } = soTang;
    public string DiaChi { get; } = diaChi;

    public string DienTichXayDungFormatted => DienTichXayDung.HasValue && DienTichXayDung.Value > 0
        ? $"{DienTichXayDung.Value} m²"
        : "";

    public bool HasDienTichXayDung => DienTichXayDung.HasValue && DienTichXayDung.Value > 0;

    public string DienTichSuDungFormatted => DienTichSuDung.HasValue && DienTichSuDung.Value > 0
        ? $"{DienTichSuDung.Value} m²"
        : "";

    public bool HasDienTichSuDung => DienTichSuDung.HasValue && DienTichSuDung.Value > 0;

    public string TaiSanCompact
    {
        get
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(TenTaiSan))
                parts.Add($"Tên tài sản: {TenTaiSan}");

            if (HasDienTichXayDung)
                parts.Add($"Diện tích xây dựng: {DienTichXayDungFormatted}");

            if (HasDienTichSuDung)
                parts.Add($"Diện tích sử dụng: {DienTichSuDungFormatted}");

            if (!string.IsNullOrWhiteSpace(SoTang))
                parts.Add($"Số tầng: {SoTang}");

            if (!string.IsNullOrWhiteSpace(DiaChi))
                parts.Add($"Địa chỉ: {DiaChi}");

            return string.Join(Environment.NewLine, parts);
        }
    }
}
