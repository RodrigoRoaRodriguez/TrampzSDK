using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;


namespace TrampzSDK
{
    /// <summary>
    /// A factory class that creates BLEButtons for the user. This factory ensures that every
    /// <code>BLEButton</code> matches a physical BLE-blutton but not that any of these are
    /// available nor within range. Also none of these buttons are neither connected nor programmed yet.
    /// </summary>
    public class BLEButtonFactory
    {
        #region Constants
        // Get a query for devices with the key service
        private String deviceSelector = GattDeviceService.GetDeviceSelectorFromUuid(Service.Keys.GetUUID(Attribute.Service));

        #endregion

        #region Fields
        //Hold information about all the devices found
        private DeviceInformationCollection deviceCollection;
        #endregion

        /// <summary>
        /// This is the constructor. Initialization performs a first scan which ensures that the device
        /// list is not null (even though it may still be empty).
        /// </summary>
        public BLEButtonFactory()
        {
            Task waitMe = Task.Run(() => Scan());

            waitMe.Wait();
        }

        #region Methods

        /// <summary>
        /// Creates a device collection from devices within reach.
        /// </summary>
        public async Task Scan()
        {
            //seek devices using the query
            deviceCollection = await DeviceInformation.FindAllAsync(deviceSelector);
        }


        /// <summary>
        /// This fuctions fetches a list of all available BLEButtons within range. Be aware that in
        /// order to interact with these buttons you will need first to call:
        /// <code>button.Connect()</code> 
        /// </summary>
        /// <returns>A list of BLEButtons</returns>
        public List<BLEButton> GetAllButtons()
        {
            List<BLEButton> buttons = new List<BLEButton>();

            //iterate through devices and create button objects
            foreach (DeviceInformation device in deviceCollection)
            {
                buttons.Add(new BLEButton(device.Id, device.Name));
            }

            return buttons;
        }

        /// <summary>
        /// Get a button by its device id.
        /// </summary>
        /// <param name="deviceId">The device id to lookup</param>
        /// <returns>A BLEButton</returns>
        public async Task<BLEButton> GetBLEButtonByDeviceId(string deviceId)
        {
            DeviceInformation deviceInfo = await DeviceInformation.CreateFromIdAsync(deviceId);
            return new BLEButton(deviceInfo.Id, deviceInfo.Name);
        }

        #endregion

    }
}