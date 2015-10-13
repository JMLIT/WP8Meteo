using System;
using Windows.UI.Xaml.Data;

namespace WP8Meteo
{
    public class BarometerNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // On récupère un double
            // Suivant sa valeur on retourne un libellé

            if (value is double)
            {
                double v = (double)value / 100.0;

                if (v < 920)
                {
                    // moins de 920 = Impossible

                    return "?";
                }
                else if (v < 980)
                {
                    // de 920 à 980 = Tempête

                    return "TEMPETE";

                }
                else if (v < 1000)
                {
                    // de 980 à 1000 = Pluie ou vent

                    return "PLUIE OU VENT";
                }
                else if (v < 1030)
                {
                    // de 1000 à 1030 = Variable

                    return "VARIABLE";
                }
                else if (v < 1050)
                {
                    // de 1030 à 1050 = Beau temps

                    return "BEAU TEMPS";
                }
                else if (v < 1070)
                {
                    // de 1050 à 1070 = Très sec

                    return "TRES SEC";
                }
                else
                {
                    // Plus de 1070 = Impossible

                    return "?";
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
