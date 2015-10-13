using System;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth;
using WP8Meteo;

namespace WP8Meteo
{
    public class BarometerSensor : SensorBase
    {
        protected double pCurrentPressure;
        protected int[] pBarometerCalibrationData = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        protected Guid BAROMETER_CALIBRATION_UUID = new Guid("F000AA43-0451-4000-B000-000000000000");

        public double CurrentPressure
        {
            get { return pCurrentPressure; }

            private set
            {
                pCurrentPressure = value;
                NotifyPropertyChanged("CurrentPressure");
            }
        }

        public BarometerSensor()
            : base("F000AA40-0451-4000-B000-000000000000", "F000AA42-0451-4000-B000-000000000000", "F000AA41-0451-4000-B000-000000000000")
        {

        }

        public override async System.Threading.Tasks.Task<SensorBase.InitResult> Init()
        {
            var r = await base.Init();

            if (r == InitResult.Ok)
            {
                // Lecture des données de calibration

                GattCommunicationStatus s;

                using (var writer = new DataWriter())
                {
                    writer.WriteByte(2);
                    s = await pConfigurationCharacteristic.WriteValueAsync(writer.DetachBuffer());
                }

                if (s == GattCommunicationStatus.Success)
                {
                    var calib = GetCharacteristic(pDeviceService, BAROMETER_CALIBRATION_UUID);

                    if (calib != null)
                    {
                        var result = await calib.ReadValueAsync(BluetoothCacheMode.Uncached);

                        if (result.Status == GattCommunicationStatus.Success && result.Value.Length == 16)
                        {
                            byte[] b = new byte[16];
                            DataReader wReader = DataReader.FromBuffer(result.Value);

                            using (wReader)
                            {
                                wReader.ReadBytes(b);

                            }

                            pBarometerCalibrationData[0] = BitConverter.ToUInt16(b, 0);
                            pBarometerCalibrationData[1] = BitConverter.ToUInt16(b, 2);
                            pBarometerCalibrationData[2] = BitConverter.ToUInt16(b, 4);
                            pBarometerCalibrationData[3] = BitConverter.ToUInt16(b, 6);
                            pBarometerCalibrationData[4] = BitConverter.ToInt16(b, 8);
                            pBarometerCalibrationData[5] = BitConverter.ToInt16(b, 10);
                            pBarometerCalibrationData[6] = BitConverter.ToInt16(b, 12);
                            pBarometerCalibrationData[7] = BitConverter.ToInt16(b, 14);
                        }
                    }
                }
            }

            return r;
        }

        protected override void OnValueChanged(IBuffer buffer)
        {
            int t_r, p_r;
            double t_a, S, O;

            DataReader wReader = DataReader.FromBuffer(buffer);

            using (wReader)
            {
                byte[] b = new byte[4];
                wReader.ReadBytes(b);

                t_r = BitConverter.ToInt16(b, 0);
                p_r = BitConverter.ToUInt16(b, 2);
            }

            t_a = (100 * (pBarometerCalibrationData[0] * t_r / Math.Pow(2, 8) + pBarometerCalibrationData[1] * Math.Pow(2, 6))) / Math.Pow(2, 16);
            S = pBarometerCalibrationData[2] + pBarometerCalibrationData[3] * t_r / Math.Pow(2, 17) + ((pBarometerCalibrationData[4] * t_r / Math.Pow(2, 15)) * t_r) / Math.Pow(2, 19);
            O = pBarometerCalibrationData[5] * Math.Pow(2, 14) + pBarometerCalibrationData[6] * t_r / Math.Pow(2, 3) + ((pBarometerCalibrationData[7] * t_r / Math.Pow(2, 15)) * t_r) / Math.Pow(2, 4);
            CurrentPressure = (S * p_r + O) / Math.Pow(2, 14);           
        }
    }
}
