using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrampzSDK
{
    /// <summary>
    /// BLEButton event listener.
    /// This class defines callbacks for various button presses on the devices
    /// associated with the BLE button and one for connection as well.
    /// </summary>
    public class BLEButtonListener
    {
        public virtual void OnLeft(BLEButton sender, DateTimeOffset timestamp) { ;}

        public virtual void OnRight(BLEButton sender, DateTimeOffset timestamp) { ;}

        public virtual void OnBoth(BLEButton sender, DateTimeOffset timestamp) { ;}

        public virtual void OnUp(BLEButton sender, DateTimeOffset timestamp) { ;}

        public virtual void OnConnect(BLEButton button) { ;}
    }
}
