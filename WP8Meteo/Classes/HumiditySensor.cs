using System;
using WP8Meteo;
using Windows.Storage.Streams;

namespace WP8Meteo
{
    public class HumiditySensor : SensorBase
    {
        protected double pCurrentHumidity;

        public double CurrentHumidity
        {
            get { return pCurrentHumidity; }

            private set
            {
                pCurrentHumidity = value;
                NotifyPropertyChanged("CurrentHumidity");
            }
        }

        public HumiditySensor()
            : base("F000AA20-0451-4000-B000-000000000000", "F000AA22-0451-4000-B000-000000000000", "F000AA21-0451-4000-B000-000000000000")
        {

        }

        protected override void OnValueChanged(IBuffer buffer)
        {
            DataReader wReader = DataReader.FromBuffer(buffer);

            using (wReader)
            {
                byte[] b = new byte[4];
                wReader.ReadBytes(b);

                int hum = BitConverter.ToUInt16(b, 2);
                hum = hum - (hum % 4);
                CurrentHumidity = -6.0 + 125.0 * (hum / 65535.0);             
            }
        }
    }
}
