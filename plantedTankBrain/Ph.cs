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
            SerialPort serial = new SerialPort("COM2", 38400, Parity.None, 8, StopBits.One);
            serial.Open();
            
            serial.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            //Thread phThread = new Thread(() => PhThread(serial));
            //phThread.Start();
        }


        //private void PhThread(SerialPort serial)
        //{

        //}

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serial = (SerialPort)sender;

            byte[] buffer = new byte[serial.BytesToRead];
            serial.Read(buffer, 0, buffer.Length);

            int carriageReturnNum = 1000;

            for (int i = 0; i < buffer.Length; i++)
            {
                Debug.Print(buffer[i].ToString());
                if (buffer[i] == 13)//13 is CR
                {
                    carriageReturnNum = i;
                }
            }

            if (carriageReturnNum == 1000)
            {
                char[] rxChar = Encoding.UTF8.GetChars(buffer);

                for (int i = 0; i < rxChar.Length; i++)
                {
                    this.buffer += rxChar[i].ToString();
                }
            }
            else
            {
                if(carriageReturnNum != 0){
                    byte[] prefix = new byte[carriageReturnNum + 1];
                    Array.Copy(buffer, prefix, carriageReturnNum + 1);
                    char[] prefixChar = Encoding.UTF8.GetChars(prefix);

                    string prefixBuffer = "";
                    for (int i = 0; i < prefixChar.Length; i++)
                    {
                        prefixBuffer += prefixChar[i].ToString();
                    }
                    Debug.Print(this.buffer + prefixBuffer);
                    this.buffer = "";
                }

                if(carriageReturnNum != buffer.Length - 1) {
                    int suffixSize = buffer.Length - (carriageReturnNum + 1);
                    byte[] suffix = new byte[suffixSize];
                    buffer.CopyTo(suffix, carriageReturnNum + 1);
                    char[] suffixChar = Encoding.UTF8.GetChars(suffix);

                    string suffixBuffer = "";
                    for (int i = 0; i < suffix.Length; i++)
                    {
                        suffixBuffer += suffixChar[i].ToString();
                    }
                    this.buffer += suffixBuffer;
                }
            }

            carriageReturnNum = 1000;
            Array.Clear(buffer, 0, buffer.Length);
        }
    }
}
