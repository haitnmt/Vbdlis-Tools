using System;

namespace Haihv.Vbdlis.Tools.Desktop.Models;

/// <summary>
/// Thông tin Giấy chứng nhận tổng hợp
/// </summary>
public record KetQuaTimKiemModel(ChuSuDungModel ChuSuDung, GiayChungNhanModel GiayChungNhanModel, ThuaDatModel ThuaDatModel, TaiSanModel TaiSan);

public record ChuSuDungModel(string HoTen, string NamSinh, string SoGiayTo, string DiaChi)
{
    public string DisplayInfo
    {
        get
        {
            var namSinhStr = string.IsNullOrWhiteSpace(NamSinh) ? "" : $"Năm sinh: {NamSinh}\n";
            var soGiayToStr = string.IsNullOrWhiteSpace(SoGiayTo) ? "" : $"Số giấy tờ: {SoGiayTo}\n";
            var diaChiStr = string.IsNullOrWhiteSpace(DiaChi) ? "" : $"Địa chỉ: {DiaChi}\n";
            return $"Họ tên: {HoTen}\n" + namSinhStr + soGiayToStr + diaChiStr;
        }
    }
}

public record GiayChungNhanModel(string SoPhatHanh, DateTime NgayCap, string SoVaoSo, DateTime NgayVaoSo)
{
    public string DisplayInfo
    {
        get
        {
            var ngayCapStr = NgayCap >= new DateTime(1993, 1, 1) ? $"Ngày cấp: {NgayCap:dd/MM/yyyy}\n" : "";
            var ngayVaoSoStr = NgayVaoSo >= new DateTime(1993, 1, 1) ? $"Ngày vào sổ: {NgayVaoSo:dd/MM/yyyy}\n" : "";
            var soVaoSoStr = string.IsNullOrWhiteSpace(SoVaoSo) ? "" : $"Số vào sổ: {SoVaoSo}\n";
            return $"Số phát hành: {SoPhatHanh}\n" + ngayCapStr + soVaoSoStr + ngayVaoSoStr;
        }
    }
}

public record ThuaDatModel(string SoToBanDo, string SoThuaDat, double DienTich, string MucDichSuDung, string DiaChi)
{
    public string DisplayInfo
    {
        get
        {
            var dienTichStr = DienTich <= 0 ? "" : $"Diện tích: {DienTich} m²\n";
            var mucDichSuDungStr = string.IsNullOrWhiteSpace(MucDichSuDung) ? "" : $"Mục đích sử dụng: {MucDichSuDung}\n";
            var diaChiStr = string.IsNullOrWhiteSpace(DiaChi) ? "" : $"Địa chỉ: {DiaChi}\n";
            return $"Số tờ bản đồ: {SoToBanDo}\n" + $"Số thửa đất: {SoThuaDat}\n" + dienTichStr + mucDichSuDungStr + diaChiStr;
        }
    }
}

public record TaiSanModel(string LoaiTaiSan, double DienTichXayDung, double DienTichSuDung, string SoTang, string DiaChi)
{
    public string DisplayInfo
    {
        get
        {
            var dienTichXayDungStr = DienTichXayDung <= 0 ? "" : $"Diện tích xây dựng: {DienTichXayDung} m²\n";
            var dienTichSuDungStr = DienTichSuDung <= 0 ? "" : $"Diện tích sử dụng: {DienTichSuDung} m²\n";
            var soTangStr = string.IsNullOrWhiteSpace(SoTang) ? "" : $"Số tầng: {SoTang}\n";
            var diaChiStr = string.IsNullOrWhiteSpace(DiaChi) ? "" : $"Địa chỉ: {DiaChi}\n";
            return $"Loại tài sản: {LoaiTaiSan}\n" + dienTichXayDungStr + dienTichSuDungStr + soTangStr + diaChiStr;
        }
    }
}
