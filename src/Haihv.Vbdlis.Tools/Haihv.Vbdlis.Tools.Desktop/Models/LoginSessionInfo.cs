namespace Haihv.Vbdlis.Tools.Desktop.Models
{
    /// <summary>
    /// Represents the latest successful login credentials and URLs kept in-memory for session refresh.
    /// </summary>
    public record LoginSessionInfo(
        string Server,
        string Username,
        string Password,
        bool HeadlessBrowser);
}
