using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

public class ChuSoHuuDto
{
    [JsonPropertyName("gioiTinh")] public int? GioiTinh { get; set; }
    [JsonPropertyName("hoTen")] public string? HoTen { get; set; }
    [JsonPropertyName("namSinh")] public string? NamSinh { get; set; }
    [JsonPropertyName("soGiayTo")] public string? SoGiayTo { get; set; }
    [JsonPropertyName("diaChi")] public string? DiaChi { get; set; }
}
