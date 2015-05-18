using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using System.Timers;

namespace TrampzSDK
{
    /// <summary>
    /// Represents a Flic button. The <code>listener</code> property determines the behaviour of the
    /// different
    /// </summary>
    public class BLEButton
    {
        private Boolean isRefreshing = false;
        Timer refreshTimer;

        /// <summary>
        /// An object that contains name, id, etc. for a BLE device.
        /// </summary>
        public readonly String Id;

        #region Properties
        private GattCharacteristic Keys { get; set; }

        /// <summary>
        /// Gets the name of this button. This is NOT an identifier. ALL Devices of the same model
        /// have the same name. Hence this can only be use to determine what kind of device you have.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The event listener for this button. In this event listener you are suppoused to define the
        /// behavior that the different clicks to have.
        /// </summary>
        public BLEButtonListener Listener { get; set; }

        #endregion

        /// <summary>
        /// This is the constructor of the button. It is INTERNAL(hence not accessible to the user) by
        /// design since we want all BLEButton objects to match physical BLE hardware. The factor
        /// ensures this is the case. 
        /// </summary>
        /// <param name="id"></param>
        internal BLEButton(String id, String name)
        {
            this.Id = id;
            this.Name = name;
            this.Listener = new BLEButtonListener();
        }

        #region Methods

        /// <summary>
        /// Connects to the BLE button. It also checks for connectivity and verifies that the
        /// physical BLE button is available.
        /// </summary>
        public async Task<bool> Connect()
        {
            refreshTimer = new Timer(5000);
            refreshTimer.AutoReset = true;
            refreshTimer.Elapsed += Refresh;

            try
            {
                // Get the Keys service from the device's id.
                GattDeviceService service = await GattDeviceService.FromIdAsync(this.Id);
                // Get the Keys data characteristic.
                this.Keys = service.GetCharacteristics(Service.Keys.GetUUID(Attribute.Data))[0];

            }
            catch (Exception) // It was impossible to even find the service
            {
                return false;
            }

            //Unsubscribe in case connect has already been run before
            this.Keys.ValueChanged -= OnPress;

            //Subscribe to listener.
            this.Keys.ValueChanged += OnPress;

            // Start refresh 
            refreshTimer.Start();

            //Enable notifications on the key characteristic.
            if (await enableNotifications()) return true; //Notifications are now enabled
            else return false;// The device was unreachable
        }

        // This connects the calls the methods in the event listener
        private void OnPress(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Task.Run(() => OnPressAsync(sender, args));
        }

        private async void OnPressAsync(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            //Create a byte array(with same size as the caracteristics value)
            Byte[] data = new byte[args.CharacteristicValue.Length];

            //Read to byte array from args
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);

            switch (data[0])
            {
                case 0:
                    this.Listener.OnUp(this, args.Timestamp); break;
                case 1:
                    this.Listener.OnRight(this, args.Timestamp); break;
                case 2:
                    this.Listener.OnLeft(this, args.Timestamp); break;
                case 3:
                    this.Listener.OnBoth(this, args.Timestamp); break;
                default:
                    throw new NotSupportedException("This device is not supported");
            }

        }


        // Tell device to notify on value change. Returns true if notifications were enabled
        // successfully
        // and false if the device was unreachable.
        private async Task<bool> enableNotifications()
        {
            GattCommunicationStatus status = await this.Keys.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);


            if (status == GattCommunicationStatus.Unreachable) return false;
            else return true;
        }

        /// <summary>
        /// Disconnect the button from the device. Disconnected buttons will not react to
        /// button presses.
        /// </summary>
        public void Disconnect()
        {
            this.refreshTimer.Stop();
            // Desubscribe from event
            this.Keys.ValueChanged -= OnPress;
            // Remove the Gatt characteristic
            this.Keys = null;
        }

        private async void Refresh(object sender, ElapsedEventArgs e)
        {
            //Unsubscribe in case connect has already been run before
            this.Keys.ValueChanged -= OnPress;
            //Get a new gatt characteristic.
            try
            {
                // Get the Keys service from the device's id.
                GattDeviceService service = await GattDeviceService.FromIdAsync(this.Id);
                // Get the Keys data characteristic.
                this.Keys = service.GetCharacteristics(Service.Keys.GetUUID(Attribute.Data))[0];

            }
            catch
            {
                throw new ApplicationException("It was impossible to refresh a BLEButton.");
            }

            //Subscribe to listener.
            this.Keys.ValueChanged += OnPress;
        }

        #endregion methods


    }
}
