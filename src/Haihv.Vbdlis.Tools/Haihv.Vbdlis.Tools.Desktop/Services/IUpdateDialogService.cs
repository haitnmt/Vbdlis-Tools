using System;
using System.Threading.Tasks;

namespace Haihv.Vbdlis.Tools.Desktop.Services
{
    public interface IUpdateDialogService
    {
        /// <summary>
        /// Shows update dialog to user and returns their choice
        /// </summary>
        /// <param name="updateInfo">Information about the available update</param>
        /// <param name="currentVersion">Current application version</param>
        /// <param name="allowLater">Whether to allow "Later" option</param>
        /// <returns>True if user chose to update now, false otherwise</returns>
        Task<bool> ShowUpdateDialogAsync(UpdateInfo updateInfo, string currentVersion, bool allowLater = true);

        /// <summary>
        /// Shows progress window for update download/installation
        /// </summary>
        /// <returns>Action to close the window</returns>
        (Action<int> UpdateProgress, Action<string> UpdateStatus, Action Close) ShowProgressWindow();
    }
}