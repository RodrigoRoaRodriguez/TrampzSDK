using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrampzSDK
{
    /// <summary>
    /// An enumerator that provides the developer with all the UUID that they will need to interact
    /// with the diffent sensors on the TI-tag.
    /// 
    /// Currently this API only uses the key service.
    /// 
    /// </summary>
    internal enum Service
    {
        // Format: Name = characters 4 to 8 of the UUID a
        Accelerometer = 0xAA10,
        Gyroscope = 0xAA50,
        Humidity = 0xAA20,
        Keys = 0xFFE0,
        Magnetometer = 0xAA30,
        Pressure = 0xAA40,
        Temperature = 0xAA00,
        TestService = 0xAA60
        // UUID for Over-The-Air-Dowload firmware service was intentionally ommited so that no one accidentally bricks a tag.
    }

    /// <summary>
    /// All the different UUID variants that yield different characteristics
    /// </summary>
    public enum Attribute
    {
        Service = 0,
        Data = 1,
        Configuration = 2,
        Frequence = 3
    }

    static class ServiceExtensions
    {
        /// <summary>
        /// The common part that all characteristic UUIDs share.
        /// The x:s are placeholders to be replaced in order to obtain a given characteristic.
        /// IMPORTANT: CASE SENSITIVE for some reason. Leads to unhandled exception otherwise.
        /// </summary>
        private const String TI_BASE_UUID = "f000XXXX-0451-4000-b000-000000000000";
        private const String TI_KEYS_UUID = "0000XXXX-0000-1000-8000-00805f9b34fb";

        public static Guid GetUUID(this Service service, Attribute attribute)
        {
            String baseUUID;

            //Set baseUUID
            if (Service.Keys == service)
                baseUUID = TI_KEYS_UUID; //Keys has a different Base UUID than all other services
            else
                baseUUID = TI_BASE_UUID;

            //Return base UUID with replacements that reflect which service and what attribute we want
            return new Guid(baseUUID.Replace("XXXX", ((int)service + (int)attribute).ToString("x4")));
        }

    }
}
