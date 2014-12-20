using System;
using System.Collections;
using System.Threading;
using System.Reflection;
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
    class Lighting
    {
        
        //start of cycle time for all of the lights... consider the values declared in the strut as defaults
        public struct Time
        {
            public static TimeSpan start = new TimeSpan(8, 15, 0);
            public static TimeSpan transition = new TimeSpan(2, 45, 0);
            public static TimeSpan photoperiod = new TimeSpan(13, 00, 0);
            public static TimeSpan overhead = new TimeSpan(7, 00, 0);
        }

        public float transition;
        public float overhead;
        public float night;
        Extender extender;

        public Lighting(Extender extender, byte pin, int frequency, float transition, float overhead, float night)
        {
            this.extender = extender;
            this.transition = transition;
            this.overhead = overhead;
            this.night = night;

            SetPwm(pin, frequency);
        }

        public Lighting(Extender extender, object pin, int frequency, float duty)
        {
            this.extender = extender;

            switch (pin)
            {
                case 7:
                    {
                        PwmOutput pwm = extender.CreatePwmOutput(GT.Socket.Pin.Seven);
                        pwm.Set(frequency, duty);
                        break;
                    }
                case 8:
                    {
                        PwmOutput pwm = extender.CreatePwmOutput(GT.Socket.Pin.Eight);
                        pwm.Set(frequency, duty);
                        break;
                    }
                case 9:
                    {
                        PwmOutput pwm = extender.CreatePwmOutput(GT.Socket.Pin.Nine);
                        pwm.Set(frequency, duty);
                        break;
                    }
            }
        }

        private void SetPwm(byte pin, int frequency)
        {
            switch (pin)
            {
                case 7:
                    {
                        PwmOutput pwm = extender.CreatePwmOutput(GT.Socket.Pin.Seven);
                        Thread threadFour = new Thread(() => PwmThread(pwm));
                        threadFour.Start();   
                        break;
                    }
                case 8:
                    {
                        PwmOutput pwm = extender.CreatePwmOutput(GT.Socket.Pin.Eight);
                        Thread threadFive = new Thread(() => PwmThread(pwm));
                        threadFive.Start();
                        break;
                    }
                case 9:
                    {
                        PwmOutput pwm = extender.CreatePwmOutput(GT.Socket.Pin.Nine);
                        Thread threadSix = new Thread(() => PwmThread(pwm));
                        threadSix.Start();
                        break;
                    }
                default:
                    {
                        Debug.Print("No or wrong pin provided for Lighting");
                        break;
                    }
            }
        }

        private void PwmThread(PwmOutput pwm)
        {
            do
            {
                float dutyCycle = GetDutyCycle(Time.start, Time.photoperiod, Time.overhead, Time.transition, this.transition, this.overhead, this.night);
                
                pwm.Set(1000, dutyCycle);

                Thread.Sleep(1000);
                string thread = Thread.CurrentThread.ManagedThreadId.ToString();
                Debug.Print("dutyCycle of " + thread + " is " + dutyCycle + " at " + DateTime.Now.TimeOfDay.ToString());
            } while (true);
        }

        // figures out what the current dutycycle should be for the pwm setting based on how far between phases the time is
        private float GetDutyCycle(TimeSpan start, TimeSpan totalLength, TimeSpan overheadLength, TimeSpan transitionLength, float transition, float overhead, float night)
        {
            float diff;
            float dutyCycle;
            long duration;
            long currentTick;

            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan sunRiseEnd = start.Add(transitionLength);
            TimeSpan morningAfternoonLength = TimeSpan.FromTicks(((totalLength.Ticks - transitionLength.Ticks * 2) - overheadLength.Ticks) / 2);
            TimeSpan sunSetStart = start.Add(totalLength).Subtract(transitionLength);
            TimeSpan sunSetEnd = start.Add(totalLength);
            TimeSpan overheadStart = TimeSpan.FromTicks(sunRiseEnd.Ticks + morningAfternoonLength.Ticks);
            TimeSpan overheadEnd = TimeSpan.FromTicks(sunSetStart.Ticks - morningAfternoonLength.Ticks);
            

            if (now > start && now < sunRiseEnd)  
            {
                diff = transition - night;
                duration = transitionLength.Ticks;
                currentTick = now.Ticks - start.Ticks;
                
                dutyCycle = night + (float)currentTick / (float)duration * diff;
                return dutyCycle;
            }
            else if (now > sunRiseEnd && now < overheadStart)
            {  
                diff = overhead - transition;
                duration = overheadStart.Ticks - sunRiseEnd.Ticks;
                currentTick = now.Ticks - sunRiseEnd.Ticks;

                dutyCycle = transition + (float)currentTick / (float)duration * diff;
                return dutyCycle;
            }
            else if (now > overheadStart && now < overheadEnd)
            {
                return overhead;
            }
            else if (now > overheadEnd && now < sunSetStart)
            {
                diff = overhead - transition;
                duration = sunSetStart.Ticks - overheadEnd.Ticks;
                currentTick = now.Ticks - overheadEnd.Ticks;

                dutyCycle = overhead - (float)currentTick / (float)duration * diff;
                return dutyCycle;
            }
            else if (now > sunSetStart && now < sunSetEnd)
            {
                diff = transition - night;
                duration = transitionLength.Ticks;
                currentTick = now.Ticks - sunSetStart.Ticks;

                dutyCycle = transition - (float)currentTick / (float)duration * diff;
                return dutyCycle;
            }
            else
            {
                return night;
            }
        }
    }
}
