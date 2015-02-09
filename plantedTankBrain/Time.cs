using System;
using System.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Time;

namespace plantedTankBrain
{
    class Time
    {
        public Time()
        {
            
        }

        public void SetTime()
        {
            Debug.Print("setting time");

            TimeService.SystemTimeChanged += new SystemTimeChangedEventHandler(TimeService_SystemTimeChanged);
            TimeService.TimeSyncFailed += new TimeSyncFailedEventHandler(TimeService_TimeSyncFailed);

            var settings = new TimeServiceSettings();
            settings.ForceSyncAtWakeUp = true;

            // refresh time is in seconds   
            settings.RefreshTime = 86400;

            settings.PrimaryServer = GetTimeServerAddress();

            TimeService.Settings = settings;
            TimeService.SetTimeZoneOffset(-300);
            TimeService.Start();
        }

        private byte[] GetTimeServerAddress()
        {
            IPAddress[] address = Dns.GetHostEntry("time.windows.com").AddressList;
            if (address != null && address.Length > 0)
            {
                return address[0].GetAddressBytes();
            }
            throw new ApplicationException("Could not get time server address");
        }

        private void TimeService_TimeSyncFailed(object sender, TimeSyncFailedEventArgs e)
        {
            Debug.Print("error synchronizing time with NTP server: " + e.ErrorCode);
        }

        private void TimeService_SystemTimeChanged(object sender, SystemTimeChangedEventArgs e)
        {
            Debug.Print("network time received. Current Date Time is " + DateTime.Now.ToString());
        }   
    }
}
