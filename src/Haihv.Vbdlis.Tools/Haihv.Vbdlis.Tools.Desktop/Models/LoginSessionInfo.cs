namespace Haihv.Vbdlis.Tools.Desktop.Models
{
    /// <summary>
    /// Represents the latest successful login credentials and URLs kept in-memory for session refresh.
    /// </summary>
    public record LoginSessionInfo(
        string Server,
        string Username,
        string Password,
        bool HeadlessBrowser)
    {
        /// <summary>
        /// URL trang Cung cấp thông tin Giấy chứng nhận
        /// </summary>
        public string CungCapThongTinGiayChungNhanPageUrl => $"{Server}/thong-tin-gcn";

        /// <summary>
        /// URL API tìm kiếm nâng cao Giấy chứng nhận
        /// </summary>
        public string AdvancedSearchGiayChungNhanUrl => $"{Server}/api/gcn/advanced-search";
    }
}
