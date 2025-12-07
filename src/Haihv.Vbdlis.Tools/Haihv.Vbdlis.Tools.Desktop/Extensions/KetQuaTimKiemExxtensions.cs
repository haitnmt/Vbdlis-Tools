using System;
using System.Collections.Generic;
using System.Linq;
using Haihv.Vbdlis.Tools.Desktop.Models;
using Haihv.Vbdlis.Tools.Desktop.ViewModels;

namespace Haihv.Vbdlis.Tools.Desktop.Extensions;

public static class KetQuaTimKiemExxtensions
{
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
