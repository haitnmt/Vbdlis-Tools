using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

public class GiayChungNhanDto
{
    [JsonPropertyName("ModifiedDate")] public string? ModifiedDate { get; set; }
    [JsonPropertyName("giayChungNhanId")] public long GiayChungNhanId { get; set; }

    [JsonPropertyName("loaiGiayChungNhanId")]
    public int? LoaiGiayChungNhanId { get; set; }

    [JsonPropertyName("quyetDinhId")] public long? QuyetDinhId { get; set; }
    [JsonPropertyName("canCuPhapLyId")] public long? CanCuPhapLyId { get; set; }
    [JsonPropertyName("canCuPhapLy")] public string? CanCuPhapLy { get; set; }
    [JsonPropertyName("daCongNhanPhapLy")] public bool? DaCongNhanPhapLy { get; set; }

    [JsonPropertyName("tinhTrangGiayChungNhan")]
    public int? TinhTrangGiayChungNhan { get; set; }

    [JsonPropertyName("soPhatHanh")] public string? SoPhatHanh { get; set; }
    [JsonPropertyName("soVaoSo")] public string? SoVaoSo { get; set; }
    [JsonPropertyName("ngayVaoSo")] public string? NgayVaoSo { get; set; }
    [JsonPropertyName("soVaoSoCu")] public string? SoVaoSoCu { get; set; }
    [JsonPropertyName("soHoSoGocCu")] public string? SoHoSoGocCu { get; set; }
    [JsonPropertyName("maVachCapTinh")] public bool? MaVachCapTinh { get; set; }
    [JsonPropertyName("maVach")] public string? MaVach { get; set; }
    [JsonPropertyName("coInMaVach")] public bool? CoInMaVach { get; set; }
    [JsonPropertyName("donViCap")] public int? DonViCap { get; set; }
    [JsonPropertyName("coQuanCap")] public string? CoQuanCap { get; set; }
    [JsonPropertyName("uyQuyenKy")] public bool? UyQuyenKy { get; set; }
    [JsonPropertyName("kyThay")] public bool? KyThay { get; set; }
    [JsonPropertyName("quyenGiamDoc")] public bool? QuyenGiamDoc { get; set; }
    [JsonPropertyName("phanCongCNVDK")] public bool? PhanCongCnvdk { get; set; }
    [JsonPropertyName("chinhQuyenDoThi")] public bool? ChinhQuyenDoThi { get; set; }

    [JsonPropertyName("tenNguoiKy")] public string? TenNguoiKy { get; set; }

    // API trả số (ví dụ: 11293) nên dùng kiểu số nguyên nullable để tương thích
    [JsonPropertyName("soHoSoGoc")] public long? SoHoSoGoc { get; set; }
    [JsonPropertyName("ghiChuTrang1")] public string? GhiChuTrang1 { get; set; }
    [JsonPropertyName("ghiChuTrang2")] public string? GhiChuTrang2 { get; set; }
    [JsonPropertyName("ghiChuTrang3")] public string? GhiChuTrang3 { get; set; }
    [JsonPropertyName("ghiChuTrang4")] public string? GhiChuTrang4 { get; set; }

    [JsonPropertyName("thayDoiTrongQuaTrinhSuDung")]
    public string? ThayDoiTrongQuaTrinhSuDung { get; set; }

    [JsonPropertyName("coInSoDo")] public bool? CoInSoDo { get; set; }

    [JsonPropertyName("inGiayNguoiDaiDien")]
    public bool? InGiayNguoiDaiDien { get; set; }

    [JsonPropertyName("inVoChong")] public bool? InVoChong { get; set; }
    [JsonPropertyName("inThuaKe")] public bool? InThuaKe { get; set; }
    [JsonPropertyName("soHopDong")] public string? SoHopDong { get; set; }
    [JsonPropertyName("ngayHopDong")] public string? NgayHopDong { get; set; }
    [JsonPropertyName("noiKyHopDong")] public string? NoiKyHopDong { get; set; }
    [JsonPropertyName("thongTinHopDong")] public string? ThongTinHopDong { get; set; }
    [JsonPropertyName("ghiChuHopDong")] public string? GhiChuHopDong { get; set; }
    [JsonPropertyName("soCongChung")] public string? SoCongChung { get; set; }
    [JsonPropertyName("ngayCongChung")] public string? NgayCongChung { get; set; }
    [JsonPropertyName("noiCongChung")] public string? NoiCongChung { get; set; }
    [JsonPropertyName("xaId")] public int? XaId { get; set; }
    [JsonPropertyName("huyenId")] public int? HuyenId { get; set; }
    [JsonPropertyName("tinhId")] public int? TinhId { get; set; }
    [JsonPropertyName("soThuaPhu")] public string? SoThuaPhu { get; set; }
    [JsonPropertyName("soToPhu")] public string? SoToPhu { get; set; }
    [JsonPropertyName("maGiayChungNhan")] public string? MaGiayChungNhan { get; set; }
    [JsonPropertyName("maQRCode")] public string? MaQrcode { get; set; }
    [JsonPropertyName("capGiayLanDau")] public bool? CapGiayLanDau { get; set; }
    [JsonPropertyName("version")] public int? Version { get; set; }
    [JsonPropertyName("isLastest")] public bool? IsLastest { get; set; }
    [JsonPropertyName("dangKyQuyen")] public string? DangKyQuyen { get; set; }
    [JsonPropertyName("soBienNhan")] public string? SoBienNhan { get; set; }
    [JsonPropertyName("nguoiDaiDienId")] public long? NguoiDaiDienId { get; set; }

    [JsonPropertyName("versionNguoiDaiDien")]
    public int? VersionNguoiDaiDien { get; set; }

    [JsonPropertyName("maDonViInGiay")] public string? MaDonViInGiay { get; set; }
    [JsonPropertyName("nguoiNhanId")] public long? NguoiNhanId { get; set; }
    [JsonPropertyName("versionNguoiNhan")] public int? VersionNguoiNhan { get; set; }
    [JsonPropertyName("hinhThucSuDung")] public int? HinhThucSuDung { get; set; }
    [JsonPropertyName("_id")] public long? UnderscoreId { get; set; }

    [JsonPropertyName("localId")] public string? LocalId { get; set; }

    // Thay bằng DTO chi tiết để ánh xạ rõ ràng với JSON trả về từ API
    [JsonPropertyName("ListDangKyQuyen")] public List<DangKyQuyenDto>? ListDangKyQuyen { get; set; }

    [JsonPropertyName("LoaiGiayChungNhan")]
    public LoaiGiayChungNhanDetailDto? LoaiGiayChungNhan { get; set; }

    [JsonPropertyName("ThongTinQuyetDinh")]
    public object? ThongTinQuyetDinh { get; set; }

    [JsonPropertyName("ThongTinCanCuPhapLy")]
    public object? ThongTinCanCuPhapLy { get; set; }

    [JsonPropertyName("isChange")] public bool? IsChange { get; set; }

    [JsonPropertyName("ListGhiNoNghiaVuTaiChinh")]
    public object? ListGhiNoNghiaVuTaiChinh { get; set; }

    [JsonPropertyName("ListHanCheQuyen")] public object? ListHanCheQuyen { get; set; }

    [JsonPropertyName("ListNghiaVuTaiChinh")]
    public object? ListNghiaVuTaiChinh { get; set; }

    [JsonPropertyName("ListNoiDungThayDoi")]
    public object? ListNoiDungThayDoi { get; set; }

    [JsonPropertyName("ChuSoHuu")] public object? ChuSoHuu { get; set; }
    [JsonPropertyName("TaiSan")] public object? TaiSan { get; set; }
    [JsonPropertyName("bienDongId")] public long? BienDongId { get; set; }
    [JsonPropertyName("hoSoTiepNhanId")] public string? HoSoTiepNhanId { get; set; }
    [JsonPropertyName("Id")] public string? Id { get; set; }
    [JsonPropertyName("Title")] public string? Title { get; set; }
    [JsonPropertyName("Description")] public string? Description { get; set; }
    [JsonPropertyName("Name")] public string? Name { get; set; }
    [JsonPropertyName("Path")] public string? Path { get; set; }
    [JsonPropertyName("ParentPath")] public string? ParentPath { get; set; }
    [JsonPropertyName("Layer")] public string? Layer { get; set; }
    [JsonPropertyName("InId")] public string? InId { get; set; }
    [JsonPropertyName("OutId")] public string? OutId { get; set; }
}
