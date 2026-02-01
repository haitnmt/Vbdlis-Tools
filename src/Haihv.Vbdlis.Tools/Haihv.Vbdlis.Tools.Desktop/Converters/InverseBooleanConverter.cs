using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Haihv.Vbdlis.Tools.Desktop.Converters;

public class InverseBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : null;
    }
}