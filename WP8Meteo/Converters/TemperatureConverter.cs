using System;
using Windows.UI.Xaml.Data;

namespace WP8Meteo
{
    public class TemperatureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // On récupère un double
            // On retourne ce double avec seulement 1 chiffre après la virgule

            if (value is double)
            {
                double v = (double)value;

                if (double.IsNaN(v))
                {
                    return "?";
                }
                else
                {
                    return v.ToString("0.0");
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
