using System;
using System.Collections;
using System.Threading;
using System.IO.Ports;
using System.Text;
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
    class Ph
    {
        string buffer;
        public Ph(Extender uart)
        {
            //create serial connection
            SerialPort serial = new SerialPort("COM2", 38400, Parity.None, 8, StopBits.One);
            serial.Open();

            // start listenning for rx
            serial.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            //get status
            genericCommand(serial, "STATUS\r");

            //set temp
            genericCommand(serial, "T,26.0\r");

            //command
            //Thread phCal = new Thread(() => genericCommand(serial, "Cal,mid,7.00\r"));
            //phCal.Start();
        }


        //private void PhThread(SerialPort serial)
        //{

        //}

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serial = (SerialPort)sender;

            byte[] rxBytes = new byte[serial.BytesToRead];
            serial.Read(rxBytes, 0, rxBytes.Length);

            char[] rxChars = Encoding.UTF8.GetChars(rxBytes);

            for (int i = 0; i < rxChars.Length; i++)
            {
                if ((byte)rxChars[i] == 13) //13 is cariage return in ascii
                {
                    Debug.Print("PH at " + DateTime.Now + ": " + this.buffer);
                    this.buffer = "";//clear the "buffer"
                    
                }
                else
                {
                    this.buffer += rxChars[i].ToString();
                }
            }
        }

        //example string: RESPONSE,1<CR>
        public void genericCommand(SerialPort serial, string command)
        {
            Thread.Sleep(4000);
            serial.Flush();
            byte[] buffer = Encoding.UTF8.GetBytes(command);
            serial.Write(buffer, 0, buffer.Length);
            Debug.Print("Generic Command Sent");
        }
    }
}
