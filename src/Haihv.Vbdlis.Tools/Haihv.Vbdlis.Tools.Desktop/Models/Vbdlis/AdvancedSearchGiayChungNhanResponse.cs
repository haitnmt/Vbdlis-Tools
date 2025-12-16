using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Haihv.Vbdlis.Tools.Desktop.Models.Vbdlis;

/// <summary>
/// Model phản hồi cho API tìm kiếm Giấy chứng nhận theo cấu trúc JSON đã cung cấp trong issue.
/// </summary>
public class AdvancedSearchGiayChungNhanResponse
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [JsonPropertyName("data")] public List<GiayChungNhanItem> Data { get; set; } = [];

    [JsonPropertyName("recordsTotal")] public int? RecordsTotal { get; set; }

    [JsonPropertyName("recordsFiltered")] public int? RecordsFiltered { get; set; }

    // Một số API có thể trả thêm statusText
    [JsonPropertyName("statusText")] public string? StatusText { get; set; }
    public bool IsError => StatusText?.Contains("error", StringComparison.OrdinalIgnoreCase) ?? false;

    /// <summary>
    /// Deserialize JSON string sang SearchGiayChungNhanResponse
    /// </summary>
    public static AdvancedSearchGiayChungNhanResponse? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        return JsonSerializer.Deserialize<AdvancedSearchGiayChungNhanResponse>(json, options);
    }
}
