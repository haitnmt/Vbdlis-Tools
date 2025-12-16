using System;
using System.Collections.Generic;
using System.Linq;

namespace Haihv.Vbdlis.Tools.Desktop.Models;

/// <summary>
/// Thông tin Giấy chứng nhận tổng hợp
/// </summary>
public record KetQuaTimKiem(
    List<ChuSuDung> ListChuSuDung,
    GiayChungNhan GiayChungNhanModel,
    List<ThuaDat> ListThuaDat,
    List<TaiSan> ListTaiSan)
{
    /// <summary>
    /// Format tất cả chủ sử dụng với dấu phân cách ---
    /// </summary>
    public string ChuSuDungCompact
    {
        get
        {
            if (ListChuSuDung == null || ListChuSuDung.Count == 0)
                return string.Empty;

            var allParts = ListChuSuDung
                .Select(chu => chu.ChuSuDungCompact)
                .Where(s => !string.IsNullOrWhiteSpace(s));

            return string.Join($"{Environment.NewLine}---{Environment.NewLine}", allParts);
        }
    }

    /// <summary>
    /// Format tất cả thửa đất với dấu phân cách ---
    /// </summary>
    public string ThuaDatCompact
    {
        get
        {
            if (ListThuaDat == null || ListThuaDat.Count == 0)
                return string.Empty;

            var allParts = ListThuaDat
                .Select(td => td.ThuaDatCompact)
                .Where(s => !string.IsNullOrWhiteSpace(s));

            return string.Join($"{Environment.NewLine}---{Environment.NewLine}", allParts);
        }
    }

    /// <summary>
    /// Format tất cả tài sản với dấu phân cách ---
    /// </summary>
    public string TaiSanCompact
    {
        get
        {
            if (ListTaiSan == null || ListTaiSan.Count == 0)
                return string.Empty;

            var allParts = ListTaiSan
                .Select(ts => ts.TaiSanCompact)
                .Where(s => !string.IsNullOrWhiteSpace(s));

            return string.Join($"{Environment.NewLine}---{Environment.NewLine}", allParts);
        }
    }
}
