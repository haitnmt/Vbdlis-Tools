using System;

namespace Haihv.Vbdlis.Tools.Desktop.Models;

/// <summary>
/// Thông tin Giấy chứng nhận tổng hợp
/// </summary>
public record KetQuaTimKiemModel(ChuSuDungModel ChuSuDung, GiayChungNhanModel GiayChungNhanModel, ThuaDatModel ThuaDatModel, TaiSanModel TaiSan);

public record ChuSuDungModel(string HoTen, string NamSinh, string SoGiayTo, string DiaChi);
public record GiayChungNhanModel(string SoPhatHanh, DateTime NgayCap, string SoVaoSo, DateTime NgayVaoSo);
public record ThuaDatModel(string SoToBanDo, string SoThuaDat, double DienTich, string MucDichSuDung, string DiaChi);
public record TaiSanModel(string LoaiTaiSan, double DienTichXayDung, double DienTichSuDung, string SoTang, string DiaChi);
