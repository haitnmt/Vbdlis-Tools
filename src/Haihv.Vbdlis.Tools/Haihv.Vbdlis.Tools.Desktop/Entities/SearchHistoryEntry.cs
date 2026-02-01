using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Haihv.Vbdlis.Tools.Desktop.Entities;

public class SearchHistoryEntry : INotifyPropertyChanged
{
    private string? _title;
    private string? _editingTitle;
    private bool _isEditing;

    public int Id { get; set; }

    public string SearchType { get; set; } = string.Empty;

    public string SearchQuery { get; set; } = string.Empty;

    public int ResultCount { get; set; }

    public int SearchItemCount { get; set; }

    public DateTime SearchedAt { get; set; }

    public string? Title
    {
        get => _title;
        set
        {
            if (_title == value) return;
            _title = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayTitle));
        }
    }

    [NotMapped] public string DefaultTitle => "Chưa có tiêu đề";

    [NotMapped] public string DisplayTitle => string.IsNullOrWhiteSpace(Title) ? DefaultTitle : Title!;

    [NotMapped] public string DisplayTime => $"Tìm kiếm lúc {SearchedAt:HH:mm:ss dd/MM/yyyy}";

    [NotMapped]
    public string DisplayDetail =>
        $"Thông tin tìm kiếm: {SearchItemCount} - Kết quả: {ResultCount}";

    [NotMapped]
    public string? EditingTitle
    {
        get => _editingTitle;
        set
        {
            if (_editingTitle == value) return;
            _editingTitle = value;
            OnPropertyChanged();
        }
    }

    [NotMapped]
    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            if (_isEditing == value) return;
            _isEditing = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}