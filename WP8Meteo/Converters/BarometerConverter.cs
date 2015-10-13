using System;
using WP8Meteo;
using Windows.UI.Xaml.Data;

namespace WP8Meteo
{
    public class BarometerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // On récupère un double
            // Suivant sa valeur on retourne un path déterminé

            if (value is double)
            {
                double v = (double)value / 100.0;

                if (v < 920)
                {
                    // Moins de 920 = Impossible

                    return App.Current.Resources["HourglassStyle"];
                }
                else if (v < 980)
                {
                    // de 920 à 980 = Tempête

                    return App.Current.Resources["CloudCycloneStyle"];

                }
                else if (v < 1000)
                {
                    // de 980 à 1000 = Pluie ou vent

                    return App.Current.Resources["CloudThunderStyle"];
                }
                else if (v < 1030)
                {
                    // de 1000 à 1030 = Variable

                    return App.Current.Resources["CloudSunStyle"];
                }
                else if (v < 1050)
                {
                    // de 1030 à 1050 = Beau temps

                    return App.Current.Resources["SunStyle"];
                }
                else if (v < 1070)
                {
                    // de 1050 à 1070 = Très sec

                    return App.Current.Resources["SunStyle"];
                }
                else
                {
                    // plus de 1070 = Impossible

                    return App.Current.Resources["HourglassStyle"];
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
