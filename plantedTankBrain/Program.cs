﻿using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
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
            DateTime time = new DateTime(2014, 12, 12, 17, 39, 00);
            Utility.SetLocalTime(time);

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");


            Lighting royalblue = new Lighting(extender, GT.Socket.Pin.Seven, 1000, .02f, 1f, .002f);
            Lighting red = new Lighting(extender, 8, 1000, .1f, 1f, .002f);
            Lighting white = new Lighting(extender2, 8, 1000, .05f, 1f, .004f);
            //Lighting blue = new Lighting(extender2, 7, 1000, .1f, 1f, .004f);

            TimeSpan co2Start = new TimeSpan(0, 00, 0);
            TimeSpan co2End = new TimeSpan(2, 00, 0);
            Co2 co2 = new Co2(relayISOx16, co2Start, co2End);

            // turn power on
            //relayISOx16.EnableRelay(RelayISOx16.Relay.Relay_7 + RelayISOx16.Relay.Relay_8);
            Debug.Print("Program Started");
        }
    }
}