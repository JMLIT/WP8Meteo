using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace WP8Meteo
{
    public abstract class SensorBase : ObservableObject
    {
        public enum InitResult
        {
            Ok,
            DeviceNotFound,
            DeviceDisconnected,
            Error
        }

        protected Guid pServiceUUID;
        protected Guid pConfigurationUUID;
        protected Guid pDataUUID;

        protected GattDeviceService pDeviceService;
        protected GattCharacteristic pConfigurationCharacteristic;
        protected GattCharacteristic pDataCharacteristic;

        public SensorBase(string serviceUUID, string configurationUUID, string dataUUID)
        {
            pServiceUUID = new Guid(serviceUUID);
            pConfigurationUUID = new Guid(configurationUUID);
            pDataUUID = new Guid(dataUUID);
        }

        public virtual async Task<InitResult> Init()
        {
            // On recherche un device supportant le service du sensor
            // Si le device trouvé est un SensorTag on retourne le service
            // Sinon on retourne null

            var deviceService = await GetDeviceService(pServiceUUID);

            if (deviceService == null)
            {
                // Aucun SensorTag ne supporte ce service

                return InitResult.DeviceNotFound;
            }

#if WINDOWS_PHONE_APP

            if (deviceService.Device.ConnectionStatus == Windows.Devices.Bluetooth.BluetoothConnectionStatus.Disconnected)
            {
                // SensorTag déconnecté
                return InitResult.DeviceDisconnected;
            }
#endif

            // On stocke le DeviceService

            pDeviceService = deviceService;

            // On a le DeviceService, on peut maintenant se brancher sur les caractéristiques
            // Configuration et Data

            var configCharacteristic = GetCharacteristic(pDeviceService, pConfigurationUUID);

            if (configCharacteristic == null)
            {
                return InitResult.DeviceNotFound;
            }

            var dataCharacteristic = GetCharacteristic(pDeviceService, pDataUUID);

            if (dataCharacteristic == null)
            {
                return InitResult.DeviceNotFound;
            }

            // On peut stocker les caractéristiques trouvées

            pConfigurationCharacteristic = configCharacteristic;
            pDataCharacteristic = dataCharacteristic;

            // On peut se brancher sur l'événement de changement de valeur des données

            await pDataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            pDataCharacteristic.ValueChanged += pDataCharacteristic_ValueChanged;

            return InitResult.Ok;
        }

        void pDataCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            OnValueChanged(args.CharacteristicValue);
        }

        protected GattCharacteristic GetCharacteristic(GattDeviceService deviceService, Guid characteristicUUID)
        {
            var characteristics = deviceService.GetCharacteristics(characteristicUUID);

            if (characteristics.Count > 0)
            {
                return characteristics[0];
            }
            else
            {
                return null;
            }
        }

        private async Task<GattDeviceService> GetDeviceService(Guid service)
        {
            var filter = GattDeviceService.GetDeviceSelectorFromUuid(service);
            var deviceInfos = await DeviceInformation.FindAllAsync(filter);

            /*foreach (var deviceInfo in deviceInfos)
            {
                if (deviceInfo.Name == "TI BLE Sensor Tag")
                {
                    // On a trouvé un SensorTag qui implémente le service recherché

                    return await GattDeviceService.FromIdAsync(deviceInfo.Id);
                }
            }*/

            if (deviceInfos != null)
            {
                // On sélectionne le premier (et unique !) service ayant cet UUID
                var devService = await GattDeviceService.FromIdAsync(deviceInfos[0].Id);
                // on retourne ce service, ou null si pas trouvé
                return devService;
            }
            else
            {
                return null;
            }
        }

        public virtual async Task Close()
        {
            // On arrête les notifications

            if (pDataCharacteristic != null)
            {
                await pDataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                pDataCharacteristic.ValueChanged -= pDataCharacteristic_ValueChanged;
                pDataCharacteristic = null;
            }

            // On arrête le capteur

            await StopSensor();

            // On libère les pointeurs

            pConfigurationCharacteristic = null;

            if (pDeviceService != null)
            {
                pDeviceService.Dispose();
                pDeviceService = null;
            }
        }

        public virtual async Task<bool> StartSensor()
        {
            if (pConfigurationCharacteristic != null)
            {
                using (var writer = new DataWriter())
                {
                    writer.WriteByte(1);
                    var status = await pConfigurationCharacteristic.WriteValueAsync(writer.DetachBuffer());
                    return (status == GattCommunicationStatus.Success);
                }
            }

            return false;
        }

        public virtual async Task<bool> StopSensor()
        {
            if (pConfigurationCharacteristic != null)
            {
                using (var writer = new DataWriter())
                {
                    writer.WriteByte(0);
                    var status = await pConfigurationCharacteristic.WriteValueAsync(writer.DetachBuffer());
                    return (status == GattCommunicationStatus.Success);
                }
            }

            return false;
        }

        protected virtual void OnValueChanged(IBuffer buffer)
        {

        }
    }
}
