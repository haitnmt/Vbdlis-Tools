using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

// DTO cho bản ghi Đăng ký quyền trong mảng ListDangKyQuyen
public class DangKyQuyenDto
{
    [JsonPropertyName("CreatedDate")] public string? CreatedDate { get; set; }
    [JsonPropertyName("ModifiedDate")] public string? ModifiedDate { get; set; }

    [JsonPropertyName("dangKyQuyenSuDungId")]
    public long DangKyQuyenSuDungId { get; set; }

    [JsonPropertyName("tinhHinhDangKyId")] public long TinhHinhDangKyId { get; set; }
    [JsonPropertyName("loaiDoiTuong")] public int? LoaiDoiTuong { get; set; }
    [JsonPropertyName("chuSuDungId")] public long? ChuSuDungId { get; set; }
    [JsonPropertyName("caNhanId")] public long? CaNhanId { get; set; }
    [JsonPropertyName("nhomNguoiId")] public long? NhomNguoiId { get; set; }
    [JsonPropertyName("typeItem")] public int? TypeItem { get; set; }
    [JsonPropertyName("itemId")] public long? ItemId { get; set; }
    [JsonPropertyName("subItemId")] public string? SubItemId { get; set; }
    [JsonPropertyName("versionChu")] public int? VersionChu { get; set; }
    [JsonPropertyName("versionItem")] public int? VersionItem { get; set; }
    [JsonPropertyName("dangKyTaiSanId")] public long? DangKyTaiSanId { get; set; }
    [JsonPropertyName("giayChungNhanId")] public long? GiayChungNhanId { get; set; }

    [JsonPropertyName("versionGiayChungNhan")]
    public int? VersionGiayChungNhan { get; set; }

    [JsonPropertyName("hanCheQuyenId")] public long? HanCheQuyenId { get; set; }

    [JsonPropertyName("nghiaVuTaiChinhId")]
    public long? NghiaVuTaiChinhId { get; set; }

    [JsonPropertyName("CaNhan")] public CaNhanDto? CaNhan { get; set; }
    [JsonPropertyName("VoChong")] public VoChongDto? VoChong { get; set; }
    [JsonPropertyName("HoGiaDinh")] public HoGiaDinhFullDto? HoGiaDinh { get; set; }
    [JsonPropertyName("ToChuc")] public ToChucDto? ToChuc { get; set; }
    [JsonPropertyName("CongDong")] public object? CongDong { get; set; }
    [JsonPropertyName("NhomNguoi")] public object? NhomNguoi { get; set; }
    [JsonPropertyName("ThuaDat")] public ThuaDatFullInfoDto? ThuaDat { get; set; }
    [JsonPropertyName("MucDichSuDung")] public object? MucDichSuDung { get; set; }
    [JsonPropertyName("NhaRiengLe")] public NhaRiengLeDto? NhaRiengLe { get; set; }
    [JsonPropertyName("HangMucNhaRiengLe")] public object? HangMucNhaRiengLe { get; set; }
    [JsonPropertyName("CanHo")] public CanHoDto? CanHo { get; set; }
    [JsonPropertyName("HangMucSoHuuChung")] public object? HangMucSoHuuChung { get; set; }
    [JsonPropertyName("NhaChungCu")] public object? NhaChungCu { get; set; }
    [JsonPropertyName("CongTrinhXayDung")] public object? CongTrinhXayDung { get; set; }
    [JsonPropertyName("CongTrinhNgam")] public object? CongTrinhNgam { get; set; }
    [JsonPropertyName("HangMucCongTrinh")] public object? HangMucCongTrinh { get; set; }
    [JsonPropertyName("RungTrong")] public object? RungTrong { get; set; }
    [JsonPropertyName("CayLauNam")] public object? CayLauNam { get; set; }
    [JsonPropertyName("keyDangKyQuyen")] public string? KeyDangKyQuyen { get; set; }
    [JsonPropertyName("edgeId")] public string? EdgeId { get; set; }

    // Metadata chung
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
