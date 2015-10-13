using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace WP8Meteo
{
    public class BluetoothColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // On récupère un boolean
            // True = RoyalBlue
            // False = Red

            if (value is bool)
            {
                bool b = (bool)value;
                
                if (b)
                {
                    return new SolidColorBrush(Colors.RoyalBlue);
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
