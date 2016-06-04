using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace plantedTankBrain
{
    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            //Set the time current time... this needs to be re-done whenever power is lost unless a clock module is added
            
            
            
            DateTime time = new DateTime(2014, 12, 12, 7, 45, 22);
            Utility.SetLocalTime(time);


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
            
            Lighting royalblue = new Lighting(extender, GT.Socket.Pin.Seven, 1000, .005f, .1f, .00005f);
            Lighting red = new Lighting(extender, GT.Socket.Pin.Eight, 1000, .1f, .50f, .0005f);
            Lighting white = new Lighting(extender2, GT.Socket.Pin.Eight, 1000, .05f, .30f, .0005f);
            Lighting blue = new Lighting(extender2, GT.Socket.Pin.Seven, 1000, .1f, 1f, .004f);

            //Lighting white = new Lighting(extender2, GT.Socket.Pin.Eight, 1000, 1f, 1f, 1f);


            TimeSpan co2Start = new TimeSpan(1, 00, 0);
            TimeSpan co2End = new TimeSpan(1, 00, 0);
            Co2 co2 = new Co2(relayISOx16, co2Start, co2End);

            relayISOx16.EnableRelay(RelayISOx16.Relays.Relay1);
            relayISOx16.EnableRelay(RelayISOx16.Relays.Relay12);

            //string[] dns = {"192.168.0.4"};

            //ethernetENC28.UseThisNetworkInterface();
            //ethernetENC28.UseStaticIP("192.168.0.109","255.255.255.0","192.168.0.1",dns);
            //////ethernetENC28.UseDHCP();
            

            //while (ethernetENC28.NetworkInterface.IPAddress == "0.0.0.0")
            //{
            //    Debug.Print("Waiting for DHCP");
            //    Thread.Sleep(250);
            //}
            //Debug.Print("IP is: " + ethernetENC28.NetworkInterface.IPAddress);

            ////check time
            //Time time = new Time();
            //time.SetTime();

            //Ph ph = new Ph(uart);


            //Debug.Print("Program Started");
            // tunes.Play(800, 1000);
            

        }
    }
}
