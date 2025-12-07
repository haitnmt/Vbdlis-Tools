using System;
using System.Collections.Generic;
using System.Linq;
using Haihv.Vbdlis.Tools.Desktop.Models;
using Haihv.Vbdlis.Tools.Desktop.ViewModels;

namespace Haihv.Vbdlis.Tools.Desktop.Extensions;

public static class KetQuaTimKiemExxtensions
{
    extension(ChuSuDungModel chuSuDungModel)
    {
        public string DisplayInfo
        {
            get
            {
                var namSinhStr = string.IsNullOrWhiteSpace(chuSuDungModel.NamSinh)
                    ? ""
                    : $"Năm sinh: {chuSuDungModel.NamSinh}\n";
                var soGiayToStr = string.IsNullOrWhiteSpace(chuSuDungModel.SoGiayTo)
                    ? ""
                    : $"Số giấy tờ: {chuSuDungModel.SoGiayTo}\n";
                var diaChiStr = string.IsNullOrWhiteSpace(chuSuDungModel.DiaChi)
                    ? ""
                    : $"Địa chỉ: {chuSuDungModel.DiaChi}\n";
                return $"Họ tên: {chuSuDungModel.HoTen}\n" +
                       namSinhStr +
                       soGiayToStr +
                       diaChiStr;
            }
        }
    }
    extension(GiayChungNhanModel giayChungNhanModel)
    {
        public string DisplayInfo
        {
            get
            {
                var ngayCapStr = giayChungNhanModel.NgayCap >= new DateTime(1993, 1, 1)
                    ? $"Ngày cấp: {giayChungNhanModel.NgayCap:dd/MM/yyyy}\n"
                    : "";
                var ngayVaoSoStr = giayChungNhanModel.NgayVaoSo >= new DateTime(1993, 1, 1)
                    ? $"Ngày vào sổ: {giayChungNhanModel.NgayVaoSo:dd/MM/yyyy}\n"
                    : "";
                var soVaoSoStr = string.IsNullOrWhiteSpace(giayChungNhanModel.SoVaoSo)
                ? ""
                : $"Số vào sổ: {giayChungNhanModel.SoVaoSo}\n";
                return $"Số phát hành: {giayChungNhanModel.SoPhatHanh}\n" +
                       ngayCapStr +
                       soVaoSoStr +
                       ngayVaoSoStr;
            }
        }
    }
    extension(ThuaDatModel thuaDatModel)
    {
        public string DisplayInfo
        {
            get
            {
                var dienTichStr = thuaDatModel.DienTich <= 0
                    ? ""
                    : $"Diện tích: {thuaDatModel.DienTich} m²\n";
                var mucDichSuDungStr = string.IsNullOrWhiteSpace(thuaDatModel.MucDichSuDung)
                    ? ""
                    : $"Mục đích sử dụng: {thuaDatModel.MucDichSuDung}\n";
                var diaChiStr = string.IsNullOrWhiteSpace(thuaDatModel.DiaChi)
                    ? ""
                    : $"Địa chỉ: {thuaDatModel.DiaChi}\n";
                return $"Số tờ bản đồ: {thuaDatModel.SoToBanDo}\n" +
                       $"Số thửa đất: {thuaDatModel.SoThuaDat}\n" +
                       dienTichStr +
                       mucDichSuDungStr +
                       diaChiStr;
            }
        }
    }
    extension(TaiSanModel taiSanModel)
    {
        public string DisplayInfo
        {
            get
            {
                var dienTichXayDungStr = taiSanModel.DienTichXayDung <= 0
                    ? ""
                    : $"Diện tích xây dựng: {taiSanModel.DienTichXayDung} m²\n";
                var dienTichSuDungStr = taiSanModel.DienTichSuDung <= 0
                    ? ""
                    : $"Diện tích sử dụng: {taiSanModel.DienTichSuDung} m²\n";
                var soTangStr = string.IsNullOrWhiteSpace(taiSanModel.SoTang)
                    ? ""
                    : $"Số tầng: {taiSanModel.SoTang}\n";
                var diaChiStr = string.IsNullOrWhiteSpace(taiSanModel.DiaChi)
                    ? ""
                    : $"Địa chỉ: {taiSanModel.DiaChi}\n";
                return $"Loại tài sản: {taiSanModel.LoaiTaiSan}\n" +
                       dienTichXayDungStr +
                       dienTichSuDungStr +
                       soTangStr +
                       diaChiStr;
            }
        }
    }
    extension(AdvancedSearchGiayChungNhanResponse giayChungNhanResponse)
    {
        public List<KetQuaTimKiemModel> ToKetQuaTimKiemModels()
        {
            var results = new List<KetQuaTimKiemModel>();

            foreach (var item in giayChungNhanResponse.Data)
            {
                // Lấy thông tin Giấy chứng nhận
                var giayChungNhan = item.GiayChungNhan;
                var giayChungNhanModel = new GiayChungNhanModel(
                    SoPhatHanh: giayChungNhan?.SoPhatHanh ?? "",
                    NgayCap: DateTime.TryParse(giayChungNhan?.ModifiedDate, out var ngayCap) ? ngayCap : DateTime.MinValue,
                    SoVaoSo: giayChungNhan?.SoVaoSo ?? "",
                    NgayVaoSo: DateTime.TryParse(giayChungNhan?.NgayVaoSo, out var ngayVaoSo) ? ngayVaoSo : DateTime.MinValue
                );

                // Lấy thông tin chủ sử dụng
                var chuSuDung = item.ChuSoHuu.FirstOrDefault();
                var chuSuDungModel = new ChuSuDungModel(
                    HoTen: chuSuDung?.HoTen ?? "",
                    NamSinh: chuSuDung?.NamSinh ?? "",
                    SoGiayTo: chuSuDung?.SoGiayTo ?? "",
                    DiaChi: chuSuDung?.DiaChi ?? ""
                );

                // Lấy thông tin tài sản
                var taiSan = item.TaiSan.FirstOrDefault();
                var thuaDatModel = new ThuaDatModel(
                    SoToBanDo: taiSan?.SoHieuToBanDo?.ToString() ?? "",
                    SoThuaDat: taiSan?.SoThuTuThua?.ToString() ?? "",
                    DienTich: 0,
                    MucDichSuDung: "",
                    DiaChi: taiSan?.DiaChi ?? ""
                );

                var taiSanModel = new TaiSanModel(
                    LoaiTaiSan: "",
                    DienTichXayDung: 0,
                    DienTichSuDung: 0,
                    SoTang: "",
                    DiaChi: taiSan?.DiaChi ?? ""
                );

                results.Add(new KetQuaTimKiemModel(
                    ChuSuDung: chuSuDungModel,
                    GiayChungNhanModel: giayChungNhanModel,
                    ThuaDatModel: thuaDatModel,
                    TaiSan: taiSanModel
                ));
            }

            return results;
        }

    }
}
