using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.SocketInterfaces;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace plantedTankBrain
{
    class Co2
    {
        public Co2(RelayISOx16 relay, TimeSpan startOffset, TimeSpan endOffset)
        {
            TimeSpan start = Lighting.Time.start.Subtract(startOffset);
            TimeSpan end = Lighting.Time.start.Add(Lighting.Time.photoperiod).Subtract(endOffset);

            Thread co2Thread = new Thread(() => Co2Thread(relay, start, end));
            co2Thread.Start();
        }

        private void Co2Thread(RelayISOx16 relay, TimeSpan start, TimeSpan end)
        {
            do
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                if (now > start && now < end)
                {
                    relay.EnableRelay(RelayISOx16.Relays.Relay7);
                }
                else
                {
                    relay.DisableRelay(RelayISOx16.Relays.Relay7);
                }

                Thread.Sleep(10000);
            } while (true);
        }
    }
}
