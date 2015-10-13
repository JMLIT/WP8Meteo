using System;
using Windows.UI.Xaml.Data;

namespace WP8Meteo
{
    public class PressureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // On récupère un double correspondant à une valeur en Pa (pascal)
            // On la divise par 100 pour obtenir des hPa
            // On retourne le résultat sans aucun chiffre après la virgule

            if (value is double)
            {
                double v = (double)value / 100.0;

                if (double.IsNaN(v))
                {
                    return "?";
                }
                else
                {
                    return v.ToString("0");
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
