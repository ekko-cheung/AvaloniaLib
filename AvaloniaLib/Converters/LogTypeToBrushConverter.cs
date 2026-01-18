using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using AvaloniaLib.Models;

namespace AvaloniaLib.Converters;

public class LogTypeToBrushConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not LogType lt)
            return Brushes.White;

        return lt switch
        {
            LogType.Debug => Brushes.LightSlateGray,
            LogType.Info => Brushes.Black,
            LogType.Warning => Brushes.Orange,
            LogType.Error => Brushes.IndianRed,
            _ => Brushes.White
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}