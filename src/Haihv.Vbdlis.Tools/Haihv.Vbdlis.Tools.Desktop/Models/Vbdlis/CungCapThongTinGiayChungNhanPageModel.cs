using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;


public static class CungCapThongTinGiayChungNhanPayload
{
    /// <summary>
    /// Tạo payload cho tìm kiếm nâng cao thông tin giấy chứng nhận.
    /// </summary>
    /// <param name="soPhatHanh">Số phát hành của giấy chứng nhận.</param>
    /// <param name="soGiayTo">Số giấy tờ của Chủ sử dụng.</param>
    /// <param name="tinhId">Mã tỉnh, mặc định là 24 (Bắc Ninh mới).</param>
    /// <returns>Chuỗi payload đã được định dạng cho việc tìm kiếm nâng cao.</returns>
    /// <exception cref="ArgumentNullException">Nếu cả <paramref name="soPhatHanh"/> và <paramref name="soGiayTo"/> đều null.</exception>
    public static string GetAdvancedSearchGiayChungNhanPayload(string? soPhatHanh = null, string? soGiayTo = null,
        int tinhId = 24)
    {
        if (tinhId <= 0) tinhId = 24;
        if (string.IsNullOrWhiteSpace(soPhatHanh) && string.IsNullOrWhiteSpace(soGiayTo))
            throw new ArgumentNullException(nameof(soPhatHanh), "Số Phát Hành hoặc Số Giấy Tờ không được để trống");
        var formData = new StringBuilder();
        // Payload đúng theo mô tả trong issue
        formData.Append("draw=2&");
        formData.Append("columns%5B0%5D%5Bdata%5D=&");
        formData.Append("columns%5B0%5D%5Bname%5D=&");
        formData.Append("columns%5B0%5D%5Bsearchable%5D=true&");
        formData.Append("columns%5B0%5D%5Borderable%5D=false&");
        formData.Append("columns%5B0%5D%5Bsearch%5D%5Bvalue%5D=&");
        formData.Append("columns%5B0%5D%5Bsearch%5D%5Bregex%5D=false&");
        formData.Append("columns%5B1%5D%5Bdata%5D=GiayChungNhan&");
        formData.Append("columns%5B1%5D%5Bname%5D=GiayChungNhan&");
        formData.Append("columns%5B1%5D%5Bsearchable%5D=true&");
        formData.Append("columns%5B1%5D%5Borderable%5D=false&");
        formData.Append("columns%5B1%5D%5Bsearch%5D%5Bvalue%5D=&");
        formData.Append("columns%5B1%5D%5Bsearch%5D%5Bregex%5D=false&");
        formData.Append("columns%5B2%5D%5Bdata%5D=ChuSoHuu&");
        formData.Append("columns%5B2%5D%5Bname%5D=ChuSoHuu&");
        formData.Append("columns%5B2%5D%5Bsearchable%5D=true&");
        formData.Append("columns%5B2%5D%5Borderable%5D=false&");
        formData.Append("columns%5B2%5D%5Bsearch%5D%5Bvalue%5D=&");
        formData.Append("columns%5B2%5D%5Bsearch%5D%5Bregex%5D=false&");
        formData.Append("columns%5B3%5D%5Bdata%5D=TaiSan&");
        formData.Append("columns%5B3%5D%5Bname%5D=TaiSan&");
        formData.Append("columns%5B3%5D%5Bsearchable%5D=true&");
        formData.Append("columns%5B3%5D%5Borderable%5D=false&");
        formData.Append("columns%5B3%5D%5Bsearch%5D%5Bvalue%5D=&");
        formData.Append("columns%5B3%5D%5Bsearch%5D%5Bregex%5D=false&");
        formData.Append("start=0&");
        formData.Append("length=10&");
        formData.Append("search%5Bvalue%5D=&");
        formData.Append("search%5Bregex%5D=false&");
        formData.Append("isAdvancedSearch=true&");
        formData.Append($"tinhId={tinhId}&");
        formData.Append("xaId=0&");
        formData.Append("huyenId=0&");
        formData.Append("timChinhXac=true&");
        formData.Append("andOperator=false&");
        formData.Append("loaiGiayChungNhanId=&");
        formData.Append("maVach=&");
        if (!string.IsNullOrWhiteSpace(soPhatHanh))
        {
            formData.Append($"soPhatHanh={WebUtility.UrlEncode(soPhatHanh)}&");
        }
        else
        {
            formData.Append("soPhatHanh=&");
        }

        formData.Append("soVaoSo=&");
        formData.Append("soHoSoGoc=&");
        formData.Append("soHoSoGocCu=&");
        formData.Append("soVaoSoCu=&");
        formData.Append("hoTen=&");
        formData.Append("namSinh=&");
        if (!string.IsNullOrWhiteSpace(soGiayTo))
        {
            formData.Append($"soGiayTo={WebUtility.UrlEncode(soGiayTo)}&");
        }
        else
        {
            formData.Append("soGiayTo=&");
        }

        formData.Append("soThuTuThua=&");
        formData.Append("soHieuToBanDo=&");
        formData.Append("soThuTuThuaCu=&");
        formData.Append("soHieuToBanDoCu=&");
        formData.Append("soNha=&");
        formData.Append("diaChiChiTiet=");

        return formData.ToString();
    }

    /// <summary>
    /// Tạo payload cho tìm kiếm nâng cao thông tin giấy chứng nhận.
    /// </summary>
    /// <param name="thuTuThua">Số thứ tự thửa của giấy chứng nhận.</param>
    /// <param name="toBanDo">Số tờ bản đồ của giấy chứng nhận.</param>
    /// <param name="xaId">Xã ID của giấy chứng nhận.</param>
    /// <param name="tinhId">Tỉnh ID của giấy chứng nhận. Mặc định là 24 (Tỉnh Bắc Ninh mới - Tỉnh Bắc Giang cũ).</param>
    /// <returns>Chuỗi payload đã được định dạng cho việc tìm kiếm nâng cao.</returns>
    /// <exception cref="ArgumentNullException">
    /// Nếu cả <paramref name="xaId"/>, <paramref name="toBanDo"/>, <paramref name="thuTuThua"/>, <paramref name="tinhId"/> đều nhỏ hơn hoặc bằng 0.</exception>
    public static string GetAdvancedSearchGiayChungNhanPayload(int thuTuThua, int toBanDo, int xaId, int tinhId = 24)
    {
        if (tinhId <= 0)
            tinhId = 24;
        if (toBanDo <= 0 || thuTuThua <= 0 || xaId <= 0)
            throw new ArgumentNullException(nameof(xaId), "Xã ID, Số Thửa, Số Tờ phải lớn hơn 0");
        var formData = new StringBuilder();
        // Payload đúng theo mô tả trong issue
        formData.Append("draw=2&");
        formData.Append("columns%5B0%5D%5Bdata%5D=&");
        formData.Append("columns%5B0%5D%5Bname%5D=&");
        formData.Append("columns%5B0%5D%5Bsearchable%5D=true&");
        formData.Append("columns%5B0%5D%5Borderable%5D=false&");
        formData.Append("columns%5B0%5D%5Bsearch%5D%5Bvalue%5D=&");
        formData.Append("columns%5B0%5D%5Bsearch%5D%5Bregex%5D=false&");
        formData.Append("columns%5B1%5D%5Bdata%5D=GiayChungNhan&");
        formData.Append("columns%5B1%5D%5Bname%5D=GiayChungNhan&");
        formData.Append("columns%5B1%5D%5Bsearchable%5D=true&");
        formData.Append("columns%5B1%5D%5Borderable%5D=false&");
        formData.Append("columns%5B1%5D%5Bsearch%5D%5Bvalue%5D=&");
        formData.Append("columns%5B1%5D%5Bsearch%5D%5Bregex%5D=false&");
        formData.Append("columns%5B2%5D%5Bdata%5D=ChuSoHuu&");
        formData.Append("columns%5B2%5D%5Bname%5D=ChuSoHuu&");
        formData.Append("columns%5B2%5D%5Bsearchable%5D=true&");
        formData.Append("columns%5B2%5D%5Borderable%5D=false&");
        formData.Append("columns%5B2%5D%5Bsearch%5D%5Bvalue%5D=&");
        formData.Append("columns%5B2%5D%5Bsearch%5D%5Bregex%5D=false&");
        formData.Append("columns%5B3%5D%5Bdata%5D=TaiSan&");
        formData.Append("columns%5B3%5D%5Bname%5D=TaiSan&");
        formData.Append("columns%5B3%5D%5Bsearchable%5D=true&");
        formData.Append("columns%5B3%5D%5Borderable%5D=false&");
        formData.Append("columns%5B3%5D%5Bsearch%5D%5Bvalue%5D=&");
        formData.Append("columns%5B3%5D%5Bsearch%5D%5Bregex%5D=false&");
        formData.Append("start=0&");
        formData.Append("length=10&");
        formData.Append("search%5Bvalue%5D=&");
        formData.Append("search%5Bregex%5D=false&");
        formData.Append("isAdvancedSearch=true&");
        formData.Append($"tinhId={tinhId}&");
        formData.Append($"xaId={xaId}&");
        formData.Append("huyenId=0&");
        formData.Append("timChinhXac=true&");
        formData.Append("andOperator=false&");
        formData.Append("loaiGiayChungNhanId=&");
        formData.Append("maVach=&");
        formData.Append("soPhatHanh=&");
        formData.Append("soVaoSo=&");
        formData.Append("soHoSoGoc=&");
        formData.Append("soHoSoGocCu=&");
        formData.Append("soVaoSoCu=&");
        formData.Append("hoTen=&");
        formData.Append("namSinh=&");
        formData.Append("soGiayTo=&");
        formData.Append($"soThuTuThua={thuTuThua}&");
        formData.Append($"soHieuToBanDo={toBanDo}&");
        formData.Append("soThuTuThuaCu=&");
        formData.Append("soHieuToBanDoCu=&");
        formData.Append("soNha=&");
        formData.Append("diaChiChiTiet=");

        return formData.ToString();
    }

}
public class GiayChungNhanItem
{
    [JsonPropertyName("GiayChungNhan")] public GiayChungNhanDto? GiayChungNhan { get; set; }

    [JsonPropertyName("ChuSoHuu")] public List<ChuSoHuuDto> ChuSoHuu { get; set; } = [];

    [JsonPropertyName("TaiSan")] public List<TaiSanDto> TaiSan { get; set; } = [];
}

/// <summary>
/// Model phản hồi cho API GetGiayChungNhanBienDong
/// </summary>
public class GetGiayChungNhanBienDongResponse
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("value")] public GiayChungNhanBienDongValue? Value { get; set; }

    /// <summary>
    /// Deserialize JSON string sang GetGiayChungNhanBienDongResponse
    /// </summary>
    public static GetGiayChungNhanBienDongResponse? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        return JsonSerializer.Deserialize<GetGiayChungNhanBienDongResponse>(json, options);
    }
}

public class GiayChungNhanBienDongValue
{
    [JsonPropertyName("GiayChungNhan")] public GiayChungNhanDto? GiayChungNhan { get; set; }
}

/// <summary>
/// Model phản hồi cho API GetThongTinTapTinHoSoQuets
/// </summary>
public class GetThongTinTapTinHoSoQuetsResponse
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("value")] public List<TapTinHoSoQuetDto>? Value { get; set; }

    /// <summary>
    /// Deserialize JSON string sang GetThongTinTapTinHoSoQuetsResponse
    /// </summary>
    public static GetThongTinTapTinHoSoQuetsResponse? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        return JsonSerializer.Deserialize<GetThongTinTapTinHoSoQuetsResponse>(json, options);
    }
}

/// <summary>
/// DTO cho thông tin tập tin hồ sơ quét
/// </summary>
public class TapTinHoSoQuetDto
{
    [JsonPropertyName("Type")] public string? Type { get; set; }
    [JsonPropertyName("NodeId")] public long NodeId { get; set; }
    [JsonPropertyName("ParentId")] public long? ParentId { get; set; }
    [JsonPropertyName("Id")] public string? Id { get; set; }
    [JsonPropertyName("Name")] public string? Name { get; set; }
    [JsonPropertyName("Title")] public string? Title { get; set; }
    [JsonPropertyName("Description")] public string? Description { get; set; }
    [JsonPropertyName("Created")] public string? Created { get; set; }
    [JsonPropertyName("Creator")] public string? Creator { get; set; }
    [JsonPropertyName("CreatorId")] public long? CreatorId { get; set; }
    [JsonPropertyName("Modified")] public string? Modified { get; set; }
    [JsonPropertyName("Modifier")] public string? Modifier { get; set; }
    [JsonPropertyName("ModifierId")] public long? ModifierId { get; set; }
    [JsonPropertyName("Path")] public string? Path { get; set; }
    [JsonPropertyName("ParentIdPath")] public string? ParentIdPath { get; set; }
    [JsonPropertyName("ParentPath")] public string? ParentPath { get; set; }
    [JsonPropertyName("Status")] public string? Status { get; set; }
    [JsonPropertyName("Template")] public string? Template { get; set; }
    [JsonPropertyName("MimeType")] public MimeTypeDto? MimeType { get; set; }
    [JsonPropertyName("Properties")] public object? Properties { get; set; }
    [JsonPropertyName("Layer")] public string? Layer { get; set; }
    [JsonPropertyName("Content")] public object? Content { get; set; }
    [JsonPropertyName("IsInherited")] public bool IsInherited { get; set; }
}

/// <summary>
/// DTO cho thông tin MIME Type
/// </summary>
public class MimeTypeDto
{
    [JsonPropertyName("MimeType")] public string? MimeType { get; set; }
    [JsonPropertyName("Display")] public string? Display { get; set; }
}