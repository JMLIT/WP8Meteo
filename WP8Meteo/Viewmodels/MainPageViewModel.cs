using System.Threading.Tasks;
using WP8Meteo;

namespace WP8Meteo
{
    public class MainPageViewModel : ObservableObject
    {
        public TemperatureSensor TemperatureSensor { get; private set; }
        public BarometerSensor BarometerSensor { get; private set; }
        public HumiditySensor HumiditySensor { get; private set; }

        private bool pSensorTagConnected;

        public bool SensorTagConnected
        {
            get { return pSensorTagConnected; }
            
            set
            {
                pSensorTagConnected = value;
                NotifyPropertyChanged("SensorTagConnected");
            }
        }

        public MainPageViewModel()
        {

        }

        public async Task<bool> Init()
        {
            // On crée les 3 capteurs et on les active

            TemperatureSensor = new TemperatureSensor();
            HumiditySensor = new HumiditySensor();
            BarometerSensor = new BarometerSensor();

            var temperatureResult = await TemperatureSensor.Init();
            var humidityResult = await HumiditySensor.Init();
            var barometerResult = await BarometerSensor.Init();

            if (temperatureResult == SensorBase.InitResult.Ok)
            {
                await TemperatureSensor.StartSensor();
                await HumiditySensor.StartSensor();
                await BarometerSensor.StartSensor();

                SensorTagConnected = true;
            }

            return SensorTagConnected;
        }
    }
}
