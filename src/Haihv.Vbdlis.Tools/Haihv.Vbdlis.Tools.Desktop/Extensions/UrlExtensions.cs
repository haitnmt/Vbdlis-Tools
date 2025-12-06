using System;
using Haihv.Vbdlis.Tools.Desktop.Models;

namespace Haihv.Vbdlis.Tools.Desktop.Extensions;

public static class UrlExtensions
{
    extension(LoginSessionInfo sessionInfo)
    {
        public string GetDiaChinhUrl
            => $"{sessionInfo.BaseUrl}";
        private string BaseUrl
        {
            get
            {
                // Lấy thông tin base URL từ Server
                var uri = new Uri(sessionInfo.Server);
                return $"{uri.Scheme}://{uri.Host}";
            }
        }
        public string CungCapThongTinGiayChungNhanPageUrl
            => $"{sessionInfo.BaseUrl}/dc/CungCapThongTinGiayChungNhan/Index";

        public string AdvancedSearchGiayChungNhanUrl
            => $"{sessionInfo.BaseUrl}/dc/CungCapThongTinGiayChungNhanAjax/AdvancedSearchGiayChungNhan";
    }

    public const string AuthenBaseUrl = "https://authen.mplis.gov.vn";
    // public static string KeKhaiDangKyV2PageUrl => $"{BaseUrl}/dc/DonDangKy/KeKhaiDangKyV2";
    // public static string AdvancedSearchTinhHinhDangKyUrl => $"{BaseUrl}/dc/DangKyAjax/AdvancedSearchTinhHinhDangKy";
    // public static string DeleteDonDangKyByTinhHinhDangKyIdUrl => $"{BaseUrl}/dc/DangKyAjax/DeleteDonDangKyByTinhHinhDangKyId";
    // public static string CungCapThongTinGiayChungNhanPageUrl => $"{BaseUrl}/dc/CungCapThongTinGiayChungNhan/Index";

    // public static string AdvancedSearchGiayChungNhanUrl =>
    //     $"{BaseUrl}/dc/CungCapThongTinGiayChungNhanAjax/AdvancedSearchGiayChungNhan";

    // public static string GetThongTinDangKyByTinhHinhDangKyIdsUrl =>
    //     $"{BaseUrl}/dc/DangKyAjax/GetThongTinDangKyByTinhHinhDangKyIds";

    // public static string UpdateHoSoQuetExistFileUrl => $"{BaseUrl}/dc/HoSoQuetAjax/UpdateHoSoQuetExistFile";

    // public static string GetHoSoQuetKeKhaiByTinhHinhDangKyIdUrl =>
    //     $"{BaseUrl}/dc/HoSoQuetAjax/GetHoSoQuetKeKhaiByTinhHinhDangKyId";

    // public static string QuanLyThongTinThuaDatPageUrl =>
    //     $"{BaseUrl}/dc/quanlythongtinchuvataisan/quanlythongtinthuadat";

    // public static string AdvancedSearchThuaDatUrl =>
    //     $"{BaseUrl}/dc/TaiSanAjax/AdvancedSearchThuaDat";
    // public static string GetGiayChungNhanByThuaDatUrl =>
    //     $"{BaseUrl}/dc/QuanLyThongTinChuVaTaiSan/GetGiayChungNhanByThuaDatNId";

    // public static string XoaThuaDatUrl =>
    //     $"{BaseUrl}/dc/QuanLyThongTinChuVaTaiSan/XoaThuaDatByThuaDatNId";

}
