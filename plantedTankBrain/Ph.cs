using System;
using System.Net;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT;

using Gadgeteer.Modules.GHIElectronics;

namespace plantedTankBrain
{
    class Ph
    {

        public Ph(Extender uart)
        {
            //start thread
            Thread Ph = new Thread(new ThreadStart(PhThread));
            Ph.Start();
        }


        private void PhThread()
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
        }

        string buffer;
        double[] previousReadings = new double[60];
        int dataCount = 0;
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
                    if (dataCount > 59)
                    {
                        dataCount = 0;
                    }

                    double bufferAsNumber;
                    if (double.TryParse(this.buffer, out bufferAsNumber))
                    {
                        if (bufferAsNumber > 2 && bufferAsNumber < 15)
                        {
                            previousReadings[dataCount] = bufferAsNumber;
                            dataCount++;
                        }
                        
                    }

                    if (dataCount == 59)
                    {
                        double average = findAverage(previousReadings);
                        saveToDatabase(average);
                    }


                    this.buffer = "";//clear the "buffer"

                }
                else
                {
                    string character = rxChars[i].ToString();
                    this.buffer += character;
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

        private double findAverage(double[] numbers)
        {
            //add them all up
            double total = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                total += numbers[i];
            }
            //divide to find the average
            return total / numbers.Length;
        }

        private void saveToDatabase(double ph)
        {
            string json = "{\"statements\": [{\"statement\": \"CREATE(n:PH {props})\", \"parameters\" : { \"props\" : { \"date:\" \"" + DateTime.Today + "\", \"time:\" \"" + DateTime.Now.TimeOfDay + "\", \"PH\": \"" + ph.ToString() + "\"}}}]}";
            Debug.Print(json);
            byte[] data = Encoding.UTF8.GetBytes(json);

            var request = (HttpWebRequest)WebRequest.Create("http://s3.jpt.pvt:7474/db/data/transaction/commit");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.KeepAlive = false;

            using (var stream = request.GetRequestStream())
            {
                try
                {
                    stream.Write(data, 0, data.Length);
                }
                catch
                {
                    Debug.Print("Error writing to database");
                }
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Debug.Print(responseString);
            request.Dispose();
        }
    }
}
