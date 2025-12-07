using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Haihv.Vbdlis.Tools.Desktop.Extensions;
using Haihv.Vbdlis.Tools.Desktop.Models;
using OfficeOpenXml;

namespace Haihv.Vbdlis.Tools.Desktop.ViewModels;

public partial class KetQuaTimKiemDataGridViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<KetQuaTimKiemModel> _ketQuaTimKiemList = [];

    [ObservableProperty]
    private KetQuaTimKiemModel? _selectedItem;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isExporting;

    public KetQuaTimKiemDataGridViewModel()
    {
        // Set EPPlus license context
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    /// <summary>
    /// Cập nhật danh sách kết quả tìm kiếm
    /// </summary>
    public void UpdateData(AdvancedSearchGiayChungNhanResponse? response)
    {
        KetQuaTimKiemList.Clear();

        if (response?.Data == null || response.Data.Count == 0)
        {
            StatusMessage = "Không có dữ liệu";
            return;
        }

        // Sử dụng extension method để chuyển đổi
        var models = response.ToKetQuaTimKiemModels();
        foreach (var model in models)
        {
            KetQuaTimKiemList.Add(model);
        }

        StatusMessage = $"Tìm thấy {KetQuaTimKiemList.Count} kết quả";
    }

    /// <summary>
    /// Xuất Excel dạng Compact (KetQuaTimKiemModel)
    /// </summary>
    [RelayCommand]
    private async Task ExportExcelCompactAsync()
    {
        if (KetQuaTimKiemList.Count == 0)
        {
            StatusMessage = "Không có dữ liệu để xuất";
            return;
        }

        try
        {
            IsExporting = true;
            StatusMessage = "Đang xuất Excel...";

            var fileName = $"KetQuaTimKiem_Compact_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            await Task.Run(() =>
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Kết quả tìm kiếm");

                // Header
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Chủ sử dụng";
                worksheet.Cells[1, 3].Value = "Số phát hành";
                worksheet.Cells[1, 4].Value = "Ngày cấp";
                worksheet.Cells[1, 5].Value = "Số vào sổ";
                worksheet.Cells[1, 6].Value = "Ngày vào sổ";
                worksheet.Cells[1, 7].Value = "Số tờ bản đồ";
                worksheet.Cells[1, 8].Value = "Số thửa đất";
                worksheet.Cells[1, 9].Value = "Địa chỉ tài sản";

                // Format header
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Data
                for (int i = 0; i < KetQuaTimKiemList.Count; i++)
                {
                    var item = KetQuaTimKiemList[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = i + 1;
                    worksheet.Cells[row, 2].Value = $"{item.ChuSuDung.HoTen} - {item.ChuSuDung.SoGiayTo}";
                    worksheet.Cells[row, 3].Value = item.GiayChungNhanModel.SoPhatHanh;
                    worksheet.Cells[row, 4].Value = item.GiayChungNhanModel.NgayCap >= new DateTime(1993, 1, 1)
                        ? item.GiayChungNhanModel.NgayCap.ToString("dd/MM/yyyy")
                        : "";
                    worksheet.Cells[row, 5].Value = item.GiayChungNhanModel.SoVaoSo;
                    worksheet.Cells[row, 6].Value = item.GiayChungNhanModel.NgayVaoSo >= new DateTime(1993, 1, 1)
                        ? item.GiayChungNhanModel.NgayVaoSo.ToString("dd/MM/yyyy")
                        : "";
                    worksheet.Cells[row, 7].Value = item.ThuaDatModel.SoToBanDo;
                    worksheet.Cells[row, 8].Value = item.ThuaDatModel.SoThuaDat;
                    worksheet.Cells[row, 9].Value = item.ThuaDatModel.DiaChi;
                }

                // Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            });

            StatusMessage = $"Đã xuất file: {fileName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi xuất Excel: {ex.Message}";
        }
        finally
        {
            IsExporting = false;
        }
    }

    /// <summary>
    /// Xuất Excel đầy đủ (tất cả thuộc tính)
    /// </summary>
    [RelayCommand]
    private async Task ExportExcelFullAsync()
    {
        if (KetQuaTimKiemList.Count == 0)
        {
            StatusMessage = "Không có dữ liệu để xuất";
            return;
        }

        try
        {
            IsExporting = true;
            StatusMessage = "Đang xuất Excel đầy đủ...";

            var fileName = $"KetQuaTimKiem_Full_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            await Task.Run(() =>
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Kết quả tìm kiếm");

                // Header - Chủ sử dụng
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Họ tên";
                worksheet.Cells[1, 3].Value = "Năm sinh";
                worksheet.Cells[1, 4].Value = "Số giấy tờ";
                worksheet.Cells[1, 5].Value = "Địa chỉ chủ sử dụng";

                // Header - Giấy chứng nhận
                worksheet.Cells[1, 6].Value = "Số phát hành";
                worksheet.Cells[1, 7].Value = "Ngày cấp";
                worksheet.Cells[1, 8].Value = "Số vào sổ";
                worksheet.Cells[1, 9].Value = "Ngày vào sổ";

                // Header - Thửa đất
                worksheet.Cells[1, 10].Value = "Số tờ bản đồ";
                worksheet.Cells[1, 11].Value = "Số thửa đất";
                worksheet.Cells[1, 12].Value = "Diện tích";
                worksheet.Cells[1, 13].Value = "Mục đích sử dụng";
                worksheet.Cells[1, 14].Value = "Địa chỉ thửa đất";

                // Header - Tài sản
                worksheet.Cells[1, 15].Value = "Loại tài sản";
                worksheet.Cells[1, 16].Value = "Diện tích xây dựng";
                worksheet.Cells[1, 17].Value = "Diện tích sử dụng";
                worksheet.Cells[1, 18].Value = "Số tầng";
                worksheet.Cells[1, 19].Value = "Địa chỉ tài sản";

                // Format header
                using (var range = worksheet.Cells[1, 1, 1, 19])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.WrapText = true;
                }

                // Data
                for (int i = 0; i < KetQuaTimKiemList.Count; i++)
                {
                    var item = KetQuaTimKiemList[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = i + 1;
                    worksheet.Cells[row, 2].Value = item.ChuSuDung.HoTen;
                    worksheet.Cells[row, 3].Value = item.ChuSuDung.NamSinh;
                    worksheet.Cells[row, 4].Value = item.ChuSuDung.SoGiayTo;
                    worksheet.Cells[row, 5].Value = item.ChuSuDung.DiaChi;

                    // Giấy chứng nhận
                    worksheet.Cells[row, 6].Value = item.GiayChungNhanModel.SoPhatHanh;
                    worksheet.Cells[row, 7].Value = item.GiayChungNhanModel.NgayCap >= new DateTime(1993, 1, 1)
                        ? item.GiayChungNhanModel.NgayCap.ToString("dd/MM/yyyy")
                        : "";
                    worksheet.Cells[row, 8].Value = item.GiayChungNhanModel.SoVaoSo;
                    worksheet.Cells[row, 9].Value = item.GiayChungNhanModel.NgayVaoSo >= new DateTime(1993, 1, 1)
                        ? item.GiayChungNhanModel.NgayVaoSo.ToString("dd/MM/yyyy")
                        : "";

                    // Thửa đất
                    worksheet.Cells[row, 10].Value = item.ThuaDatModel.SoToBanDo;
                    worksheet.Cells[row, 11].Value = item.ThuaDatModel.SoThuaDat;
                    worksheet.Cells[row, 12].Value = item.ThuaDatModel.DienTich > 0 ? item.ThuaDatModel.DienTich : (object)"";
                    worksheet.Cells[row, 13].Value = item.ThuaDatModel.MucDichSuDung;
                    worksheet.Cells[row, 14].Value = item.ThuaDatModel.DiaChi;

                    // Tài sản
                    worksheet.Cells[row, 15].Value = item.TaiSan.LoaiTaiSan;
                    worksheet.Cells[row, 16].Value = item.TaiSan.DienTichXayDung > 0 ? item.TaiSan.DienTichXayDung : (object)"";
                    worksheet.Cells[row, 17].Value = item.TaiSan.DienTichSuDung > 0 ? item.TaiSan.DienTichSuDung : (object)"";
                    worksheet.Cells[row, 18].Value = item.TaiSan.SoTang;
                    worksheet.Cells[row, 19].Value = item.TaiSan.DiaChi;
                }

                // Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            });

            StatusMessage = $"Đã xuất file: {fileName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi xuất Excel: {ex.Message}";
        }
        finally
        {
            IsExporting = false;
        }
    }
}
