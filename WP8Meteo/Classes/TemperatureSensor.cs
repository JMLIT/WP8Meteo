using System;
using Windows.Storage.Streams;

namespace WP8Meteo
{
    public class TemperatureSensor : SensorBase
    {
        protected double pCurrentTemperature;

        public double CurrentTemperature
        {
            get { return pCurrentTemperature; }

            private set
            {
                pCurrentTemperature = value;
                NotifyPropertyChanged("CurrentTemperature");
            }
        }

        public TemperatureSensor()
            : base("F000AA00-0451-4000-B000-000000000000", "F000AA02-0451-4000-B000-000000000000", "F000AA01-0451-4000-B000-000000000000")
        {

        }

        protected override void OnValueChanged(IBuffer buffer)
        {
            DataReader wReader = DataReader.FromBuffer(buffer);

            using (wReader)
            {
                byte[] b = new byte[4];
                wReader.ReadBytes(b);

                var ambientTemperature = BitConverter.ToInt16(b, 2) / 128.0;

                double Vobj2 = BitConverter.ToInt16(b, 0);
                Vobj2 *= 0.00000015625;

                double Tdie = ambientTemperature + 273.15;

                double S0 = 5.593E-14;
                double a1 = 1.75E-3;
                double a2 = -1.678E-5;
                double b0 = -2.94E-5;
                double b1 = -5.7E-7;
                double b2 = 4.63E-9;
                double c2 = 13.4;
                double Tref = 298.15;
                double S = S0 * (1 + a1 * (Tdie - Tref) + a2 * Math.Pow((Tdie - Tref), 2));
                double Vos = b0 + b1 * (Tdie - Tref) + b2 * Math.Pow((Tdie - Tref), 2);
                double fObj = (Vobj2 - Vos) + c2 * Math.Pow((Vobj2 - Vos), 2);
                double tObj = Math.Pow(Math.Pow(Tdie, 4) + (fObj / S), .25);

                //CurrentTemperature = tObj - 273.15;
                // On retourne plutôt le température ambiante, plus réaliste que temp courante.
                CurrentTemperature = ambientTemperature;
            }
        }
    }
}
