using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

namespace Haihv.Vbdlis.Tools.Desktop.Models;

public class ChuSuDung
{
    public CaNhanDto? CaNhan { get; init; }
    public VoChongDto? VoChong { get; init; }
    public HoGiaDinhFullDto? HoGiaDinh { get; init; }
    public ToChucDto? ToChuc { get; init; }
    public string? CongDong { get; init; }
    public string? NhomNguoi { get; init; }

    private static string? FormatSoGiayTo(IEnumerable<GiayToTuyThanDto>? giayToList, bool showDashIfEmpty = true)
    {
        if (giayToList == null)
            return showDashIfEmpty ? "(-)" : null;
        var giayToTuyThanDtos = giayToList.ToList();
        var allNumbers = giayToTuyThanDtos
            .Where(g => !string.IsNullOrWhiteSpace(g.SoGiayTo))
            .OrderByDescending(g => g.LaThongTinChinh == true)
            .Select(g => g.SoGiayTo!.Trim())
            .ToList();

        if (allNumbers.Count == 0)
            return showDashIfEmpty ? "(-)" : null;

        var distinctNumbers = allNumbers.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var first = distinctNumbers[0];
        var loaiGiayTo = giayToTuyThanDtos
            .FirstOrDefault(g =>
                !string.IsNullOrWhiteSpace(g.SoGiayTo) &&
                string.Equals(g.SoGiayTo!.Trim(), first, StringComparison.OrdinalIgnoreCase))
            ?.LoaiGiayToTuyThan;
        var label = loaiGiayTo?.MaLoaiGiayTo ?? loaiGiayTo?.TenLoaiGiayTo;
        var firtText = string.IsNullOrWhiteSpace(label) ? $"Số giấy tờ: {first}" : $"{label}: {first}";
        return distinctNumbers.Count == 1
            ? firtText
            : firtText + string.Concat(distinctNumbers.Skip(1).Select(n => $"; ({n})"));
    }

    private static string? FormatSoGiayTo(IEnumerable<GiayToToChucDto>? giayToToChucList, bool showDashIfEmpty)
    {
        if (giayToToChucList == null)
            return showDashIfEmpty ? "(-)" : null;

        var grouped = new Dictionary<string, List<GiayToToChucDto>>(StringComparer.OrdinalIgnoreCase);
        foreach (var giayTo in giayToToChucList)
        {
            if (string.IsNullOrWhiteSpace(giayTo.SoGiayTo))
                continue;

            var key = (GetLoaiGiayToToChucText(giayTo) ?? "").Trim();
            if (!grouped.TryGetValue(key, out var list))
            {
                list = [];
                grouped[key] = list;
            }

            list.Add(giayTo);
        }

        if (grouped.Count == 0)
            return showDashIfEmpty ? "(-)" : null;

        var parts = (from @group in grouped
            let label = string.IsNullOrWhiteSpace(@group.Key) ? "Số giấy tờ:" : @group.Key
            let numbers = FormatSoGiayToNumbers(@group.Value.Select(g => g.SoGiayTo), showDashIfEmpty: false)
            where !string.IsNullOrWhiteSpace(numbers)
            select $"{label}: {numbers}").ToList();

        if (parts.Count == 0)
            return showDashIfEmpty ? "(-)" : null;

        return string.Join("; ", parts);
    }

    private static string? GetLoaiGiayToToChucText(GiayToToChucDto giayTo)
    {
        var value = giayTo.LoaiGiayToToChuc;
        switch (value)
        {
            case null:
                break;
            case string str:
                return str;
            case JsonElement { ValueKind: JsonValueKind.String } element:
                return element.GetString();
            case JsonElement element:
            {
                if (element.ValueKind != JsonValueKind.Object) return null;
                foreach (var key in new[]
                         {
                             "tenLoaiGiayToToChuc", "TenLoaiGiayToToChuc", "tenLoaiGiayTo", "TenLoaiGiayTo", "ten",
                             "Ten", "name", "Name", "title", "Title"
                         })
                {
                    if (TryGetJsonString(element, key, out var text))
                        return text;
                }

                break;
            }
            case IDictionary<string, object?> dict:
            {
                foreach (var key in new[]
                         {
                             "tenLoaiGiayToToChuc", "TenLoaiGiayToToChuc", "tenLoaiGiayTo", "TenLoaiGiayTo", "ten",
                             "Ten", "name", "Name", "title", "Title"
                         })
                {
                    if (!dict.TryGetValue(key, out var raw) || raw == null)
                        continue;

                    switch (raw)
                    {
                        case string text when !string.IsNullOrWhiteSpace(text):
                            return text;
                        case JsonElement { ValueKind: JsonValueKind.String } rawElement:
                            return rawElement.GetString();
                    }
                }

                break;
            }
        }

        return null;
    }

    private static bool TryGetJsonString(JsonElement obj, string propertyName, out string? value)
    {
        value = null;

        if (!obj.TryGetProperty(propertyName, out var property))
            return false;

        if (property.ValueKind != JsonValueKind.String)
            return false;

        value = property.GetString();
        return !string.IsNullOrWhiteSpace(value);
    }

    private static string? FormatSoGiayToNumbers(IEnumerable<string?>? soGiayToList, bool showDashIfEmpty)
    {
        if (soGiayToList == null)
            return showDashIfEmpty ? "(-)" : null;

        var allNumbers = soGiayToList
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!.Trim())
            .ToList();

        if (allNumbers.Count == 0)
            return showDashIfEmpty ? "(-)" : null;

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var distinctNumbers = new List<string>();
        foreach (var number in allNumbers)
        {
            if (seen.Add(number))
                distinctNumbers.Add(number);
        }

        var first = distinctNumbers[0];
        return distinctNumbers.Count == 1
            ? first
            : first + string.Concat(distinctNumbers.Skip(1).Select(n => $"; ({n})"));
    }

    public string TenChu(bool includeTitle = true)
    {
        static string OngBa(bool? gioiTinh, bool capitalize)
        {
            if (gioiTinh == true)
                return capitalize ? "Ông" : "ông";

            return capitalize ? "Bà" : "bà";
        }

        var parts = new List<string>();

        if (CaNhan != null && !string.IsNullOrWhiteSpace(CaNhan.HoTen))
            parts.Add(
                $"{(includeTitle ? "Họ tên: " : "")}{OngBa(CaNhan.GioiTinh, capitalize: !includeTitle)} {CaNhan.HoTen}");

        if (VoChong != null)
        {
            var tenChong = VoChong.Chong?.HoTen ?? "";
            var tenVo = VoChong.Vo?.HoTen ?? "";

            if (!string.IsNullOrWhiteSpace(tenChong) && !string.IsNullOrWhiteSpace(tenVo))
            {
                var tenVoChong = VoChong.InVoTruoc == true
                    ? $"{(includeTitle ? "Họ tên: " : "")}vợ {tenVo} và chồng {tenChong}"
                    : $"{(includeTitle ? "Họ tên: " : "")}chồng {tenChong} và vợ {tenVo}";
                parts.Add(tenVoChong);
            }
            else if (!string.IsNullOrWhiteSpace(tenChong))
                parts.Add($"{(includeTitle ? "Họ tên: " : "")}{OngBa(true, capitalize: !includeTitle)} {tenChong}");
            else if (!string.IsNullOrWhiteSpace(tenVo))
                parts.Add($"{(includeTitle ? "Họ tên: " : "")}{OngBa(false, capitalize: !includeTitle)} {tenVo}");
        }

        if (HoGiaDinh != null)
        {
            var tenThanhVien = new List<string>();

            var tenChuHo = HoGiaDinh.ChuHo?.HoTen ?? "";
            if (!string.IsNullOrWhiteSpace(tenChuHo))
                tenThanhVien.Add($"Chủ hộ {(HoGiaDinh.ChuHo?.GioiTinh == true ? "Ông" : "Bà")} {tenChuHo}");

            if (HoGiaDinh.ListThanhVienHoGiaDinh is { Count: > 0 })
            {
                var tenCacThanhVien = HoGiaDinh.ListThanhVienHoGiaDinh
                    .Where(tv => !string.IsNullOrWhiteSpace(tv.HoTen))
                    .Select(tv => $"{OngBa(tv.GioiTinh, capitalize: !includeTitle)} {tv.HoTen!}");
                tenThanhVien.AddRange(tenCacThanhVien);
            }

            var tenHoGiaDinh = tenThanhVien.Count > 0
                ? $"{(includeTitle ? "Hộ gia đình: " : "")}({string.Join(", ", tenThanhVien)})"
                : "Hộ gia đình: (không tìm thấy thành viên)";
            parts.Add(tenHoGiaDinh);
        }

        if (ToChuc != null && !string.IsNullOrWhiteSpace(ToChuc.TenToChuc))
            parts.Add($"{(includeTitle ? "Tên tổ chức: " : "")}{ToChuc.TenToChuc}");

        if (CongDong != null && !string.IsNullOrWhiteSpace(CongDong))
            parts.Add($"{(includeTitle ? "Cộng đồng: " : "")}{CongDong}");

        if (NhomNguoi != null && !string.IsNullOrWhiteSpace(NhomNguoi))
            parts.Add($"{(includeTitle ? "Nhóm người: " : "")}{NhomNguoi}");

        return string.Join("; ", parts);
    }

    private string? NamSinh
    {
        get
        {
            var parts = new List<string>();

            if (CaNhan?.NamSinh != null)
                parts.Add(CaNhan.NamSinh.ToString()!);

            if (VoChong != null)
            {
                var namSinhChong = VoChong.Chong?.NamSinh?.ToString() ?? "";
                var namSinhVo = VoChong.Vo?.NamSinh?.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(namSinhChong) && !string.IsNullOrWhiteSpace(namSinhVo))
                {
                    var namSinhVoChong = VoChong.InVoTruoc == true
                        ? $"{namSinhVo}, {namSinhChong}"
                        : $"{namSinhChong}, {namSinhVo}";
                    parts.Add(namSinhVoChong);
                }
                else if (!string.IsNullOrWhiteSpace(namSinhChong))
                    parts.Add(namSinhChong);
                else if (!string.IsNullOrWhiteSpace(namSinhVo))
                    parts.Add(namSinhVo);
            }

            if (HoGiaDinh == null) return parts.Count > 0 ? string.Join("; ", parts) : null;
            var namSinhThanhVien = new List<string>();

            if (HoGiaDinh.ChuHo?.NamSinh != null)
                namSinhThanhVien.Add($"Chủ hộ: {HoGiaDinh.ChuHo.NamSinh}");

            if (HoGiaDinh.ListThanhVienHoGiaDinh is { Count: > 0 })
            {
                var namSinhCacThanhVien = HoGiaDinh.ListThanhVienHoGiaDinh
                    .Where(tv => tv.NamSinh != null)
                    .Select(tv => tv.NamSinh!.Value.ToString());
                namSinhThanhVien.AddRange(namSinhCacThanhVien);
            }

            if (namSinhThanhVien.Count > 0)
                parts.Add(string.Join(", ", namSinhThanhVien));

            return parts.Count > 0 ? string.Join("; ", parts) : null;
        }
    }

    public string? SoGiayTo
    {
        get
        {
            var parts = new List<string>();

            if (CaNhan != null)
            {
                parts.Add(FormatSoGiayTo(CaNhan.ListGiayToTuyThan, showDashIfEmpty: true)!);
            }

            if (VoChong != null)
            {
                var giayToChong = FormatSoGiayTo(VoChong.Chong?.ListGiayToTuyThan, showDashIfEmpty: false);
                var giayToVo = FormatSoGiayTo(VoChong.Vo?.ListGiayToTuyThan, showDashIfEmpty: false);

                switch (string.IsNullOrWhiteSpace(giayToChong))
                {
                    case false when !string.IsNullOrWhiteSpace(giayToVo):
                    {
                        var giayToVoChong = VoChong.InVoTruoc == true
                            ? $"{giayToVo}, {giayToChong}"
                            : $"{giayToChong}, {giayToVo}";
                        parts.Add(giayToVoChong);
                        break;
                    }
                    case false:
                        parts.Add(!string.IsNullOrWhiteSpace(giayToChong) ? giayToChong : "(-)");
                        break;
                    default:
                    {
                        parts.Add(!string.IsNullOrWhiteSpace(giayToVo) ? giayToVo : "(-)");
                        break;
                    }
                }
            }

            if (HoGiaDinh != null)
            {
                var giayToThanhVien = new List<string>();

                var chuHoGiayTo = FormatSoGiayTo(HoGiaDinh.ChuHo?.ListGiayToTuyThan, showDashIfEmpty: true);
                giayToThanhVien.Add($"Chủ hộ: {chuHoGiayTo}");

                if (HoGiaDinh.ListThanhVienHoGiaDinh is { Count: > 0 })
                {
                    giayToThanhVien.AddRange(HoGiaDinh.ListThanhVienHoGiaDinh
                        .Select(tv => FormatSoGiayTo(tv.ListGiayToTuyThan, showDashIfEmpty: false))
                        .Where(tvGiayTo => !string.IsNullOrWhiteSpace(tvGiayTo))!);
                }

                if (giayToThanhVien.Count > 0)
                    parts.Add(string.Join(", ", giayToThanhVien));
            }

            if (ToChuc == null) return parts.Count > 0 ? string.Join("; ", parts) : null;
            var giayToToChuc = FormatSoGiayTo(ToChuc.ListGiayToBoSung, showDashIfEmpty: false);
            if (!string.IsNullOrWhiteSpace(giayToToChuc))
                parts.Add(giayToToChuc);

            return parts.Count > 0 ? string.Join("; ", parts) : null;
        }
    }

    public string? DiaChi
    {
        get
        {
            var parts = new List<string>();

            if (CaNhan != null)
            {
                if (!string.IsNullOrWhiteSpace(CaNhan.DiaChi))
                    parts.Add(CaNhan.DiaChi);
                else
                {
                    var diaChiCaNhan = CaNhan.ListDiaChi?
                                           .FirstOrDefault(d => d.LaDiaChiChinh)?.DiaChiChiTiet
                                       ?? CaNhan.DiaChi;
                    if (!string.IsNullOrWhiteSpace(diaChiCaNhan))
                        parts.Add(diaChiCaNhan);
                }
            }

            if (VoChong != null)
            {
                var diaChiChong = string.IsNullOrWhiteSpace(VoChong.Chong?.DiaChi)
                    ? VoChong.Chong?.ListDiaChi?
                          .FirstOrDefault(d => d.LaDiaChiChinh)?.DiaChiChiTiet
                      ?? VoChong.Chong?.DiaChi ?? ""
                    : VoChong.Chong?.DiaChi ?? "";
                var diaChiVo = string.IsNullOrWhiteSpace(VoChong.Vo?.DiaChi)
                    ? VoChong.Vo?.ListDiaChi?
                          .FirstOrDefault(d => d.LaDiaChiChinh)?.DiaChiChiTiet
                      ?? VoChong.Vo?.DiaChi ?? ""
                    : VoChong.Vo?.DiaChi ?? "";

                // Nếu cả hai có địa chỉ giống nhau, chỉ thêm một
                if (!string.IsNullOrWhiteSpace(diaChiChong) && diaChiChong == diaChiVo)
                {
                    parts.Add(diaChiChong);
                }
                else if (!string.IsNullOrWhiteSpace(diaChiChong) && !string.IsNullOrWhiteSpace(diaChiVo))
                {
                    var diaChiVoChong = VoChong.InVoTruoc == true
                        ? $"{diaChiVo}; {diaChiChong}"
                        : $"{diaChiChong}; {diaChiVo}";
                    parts.Add(diaChiVoChong);
                }
                else if (!string.IsNullOrWhiteSpace(diaChiChong))
                    parts.Add(diaChiChong);
                else if (!string.IsNullOrWhiteSpace(diaChiVo))
                    parts.Add(diaChiVo);
            }

            if (HoGiaDinh != null)
            {
                // Địa chỉ hộ gia đình thường chung, nên chỉ lấy địa chỉ chung của hộ
                var diaChiHoGiaDinh = string.IsNullOrWhiteSpace(HoGiaDinh.DiaChi)
                    ? HoGiaDinh.ListDiaChi?
                          .FirstOrDefault(d => d.LaDiaChiChinh)?.DiaChiChiTiet
                      ?? HoGiaDinh.DiaChi
                    : HoGiaDinh.DiaChi;
                if (!string.IsNullOrWhiteSpace(diaChiHoGiaDinh))
                    parts.Add(diaChiHoGiaDinh);
            }

            if (ToChuc != null)
            {
                var diaChiToChuc = string.IsNullOrWhiteSpace(ToChuc.DiaChi)
                    ? ToChuc.ListDiaChi?
                          .FirstOrDefault(d => d.LaDiaChiChinh)?.DiaChiChiTiet
                      ?? ToChuc.DiaChi
                    : ToChuc.DiaChi;
                if (!string.IsNullOrWhiteSpace(diaChiToChuc))
                    parts.Add(diaChiToChuc);
            }

            return parts.Count > 0 ? string.Join("; ", parts) : null;
        }
    }

    public string ChuSuDungCompact
    {
        get
        {
            var parts = new List<string>();
            var tenChu = TenChu();
            if (!string.IsNullOrWhiteSpace(tenChu))
                parts.Add(tenChu);

            if (!string.IsNullOrWhiteSpace(NamSinh))
                parts.Add($"Năm sinh: {NamSinh}");

            if (!string.IsNullOrWhiteSpace(SoGiayTo))
                parts.Add(SoGiayTo);

            if (!string.IsNullOrWhiteSpace(DiaChi))
                parts.Add($"Địa chỉ: {DiaChi}");

            return string.Join(Environment.NewLine, parts);
        }
    }
}