using System;
using System.Threading;
using Microsoft.SPOT;

using Gadgeteer.SocketInterfaces;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace plantedTankBrain
{
    class Lighting
    {

        //photoperiod times
        public struct Time
        {
            public static TimeSpan sunrise = new TimeSpan(7, 15, 0);
            public static TimeSpan morning = new TimeSpan(7, 30, 0);
            public static TimeSpan fadeToOverhead = new TimeSpan(8, 30, 0);
            public static TimeSpan overhead = new TimeSpan(9, 00, 0);
            public static TimeSpan fadeToLowLight = new TimeSpan(15, 00, 00);
            public static TimeSpan evening = new TimeSpan(16, 45, 0);
            public static TimeSpan sunset = new TimeSpan(17, 00, 0);
            public static TimeSpan night = new TimeSpan(17, 05, 0);
        }

        //duty cycle settings for each time period... values set are just defaults
        public float overhead = .1f;
        public float lowLight = .05f;
        public float night = .02f;

        public int frequency;
        Extender extender;

        public Lighting(Extender extender, GT.Socket.Pin pin, int frequency, float lowLightDutyCycle, float overheadDutyCycle, float nightDutyCycle)
        {
            this.extender = extender;
            this.frequency = frequency;
            this.lowLight = lowLightDutyCycle;
            this.overhead = overheadDutyCycle;
            this.night = nightDutyCycle;

            //asign the pin for pwm
            PwmOutput pwm = extender.CreatePwmOutput(pin);

            //thread that loops changing the light output per time of day
            Thread thread = new Thread(() => PwmThread(pwm));
            thread.Start();
        }

        private void PwmThread(PwmOutput pwm)
        {
            do
            {
                float dutyCycle = GetCurrentDutyCycle();

                try
                {
                    pwm.Set(this.frequency, dutyCycle);
                }
                catch
                {
                    Debug.Print("Unable to set pwm");
                }

                Thread.Sleep(1000);

                //string thread = Thread.CurrentThread.ManagedThreadId.ToString();
                //Debug.Print("dutyCycle of " + thread + " is " + dutyCycle + " at " + DateTime.Now.TimeOfDay.ToString());
            } while (true);
        }

        // this one has transitions but the stages are constant
        private float GetCurrentDutyCycle()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (isCurrentTimePeriod(now, Time.sunrise, Time.morning))
            {//sunrise
                return fader(now, Time.sunrise, Time.morning, this.night, this.lowLight);
            }
            else if (isCurrentTimePeriod(now, Time.morning, Time.fadeToOverhead))
            {//morning low light
                return this.lowLight;
            }
            else if (isCurrentTimePeriod(now, Time.fadeToOverhead, Time.overhead))
            { //fading between morning and overhead
                return fader(now, Time.fadeToOverhead, Time.overhead, this.lowLight, this.overhead);
            }
            else if (isCurrentTimePeriod(now, Time.overhead, Time.fadeToLowLight))
            {//overhead/ max light
                return this.overhead;
            }
            else if (isCurrentTimePeriod(now, Time.fadeToLowLight, Time.evening))
            {// fader to evening
                return fader(now, Time.fadeToLowLight, Time.evening, this.overhead, this.lowLight);
            }
            else if (isCurrentTimePeriod(now, Time.evening, Time.sunset))
            {// evening
                return this.lowLight;
            }
            else if (isCurrentTimePeriod(now, Time.sunset, Time.night))
            { //sunset
                return fader(now, Time.sunset, Time.night, this.lowLight, this.night);
            }
            else
            {// night time
                return this.night;
            }
        }

        private bool isCurrentTimePeriod(TimeSpan now, TimeSpan start, TimeSpan finish)
        {
            if (now > start && now < finish)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private float fader(TimeSpan now, TimeSpan start, TimeSpan end, float startDutyCycle, float finishDutyCycle)
        {
            long duration;
            long currentTick;
            float dutyCycleDifference;
            float dutyCycleResult;

            if (startDutyCycle < finishDutyCycle) //math.abs does not exist in .netmf 4.3 :(
            {
                dutyCycleDifference = startDutyCycle - finishDutyCycle;
                duration = start.Ticks - end.Ticks;
                currentTick = now.Ticks - start.Ticks;
                dutyCycleResult = startDutyCycle + (float)currentTick / (float)duration * dutyCycleDifference;
            }
            else
            {
                dutyCycleDifference = finishDutyCycle - startDutyCycle;
                duration = end.Ticks - start.Ticks;
                currentTick = start.Ticks - now.Ticks;
                dutyCycleResult = startDutyCycle - (float)currentTick / (float)duration * dutyCycleDifference;
            }


            return dutyCycleResult;
        }
    }
}
