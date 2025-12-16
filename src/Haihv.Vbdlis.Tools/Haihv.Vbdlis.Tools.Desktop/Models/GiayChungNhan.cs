using System;

namespace Haihv.Vbdlis.Tools.Desktop.Models;

public class GiayChungNhan(string id, string soPhatHanh, string soVaoSo, DateTime? ngayVaoSo)
{
    public string Id { get; } = id;
    public string SoPhatHanh { get; } = soPhatHanh;
    public string SoVaoSo { get; } = soVaoSo;
    public DateTime? NgayVaoSo { get; } = ngayVaoSo;

    public string NgayVaoSoFormatted => NgayVaoSo.HasValue && NgayVaoSo.Value >= new DateTime(1900, 1, 1)
        ? NgayVaoSo.Value.ToString("dd/MM/yyyy")
        : "";

    public bool HasNgayVaoSo => NgayVaoSo.HasValue && NgayVaoSo.Value >= new DateTime(1900, 1, 1);
}
