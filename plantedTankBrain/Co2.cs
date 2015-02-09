using System;
using System.Threading;

using Gadgeteer.Modules.GHIElectronics;

namespace plantedTankBrain
{
    class Co2
    {
        public Co2(RelayISOx16 relay, TimeSpan startOffset, TimeSpan endOffset)
        {
            TimeSpan start = Lighting.Time.sunrise.Subtract(startOffset);
            TimeSpan end = Lighting.Time.fadeToLowLight.Subtract(endOffset);

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
                    relay.EnableRelay(RelayISOx16.Relays.Relay13);
                }
                else
                {
                    relay.DisableRelay(RelayISOx16.Relays.Relay13);
                }

                Thread.Sleep(1000);
            } while (true);
        }
    }
}
