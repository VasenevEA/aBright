using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aBright
{
    static class Program
    {
        public static double lux = 0;
        public static int min = 30;
        public static int max = 130;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Brightness.SetBrightness(100);
            Task.Factory.StartNew(updater);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        private static async void updater()
        {
            var regCheck = new ModbusConfig
            {
                baudRate = 9600,
                device_address = 1,
                portName = "COM6",
                register_read_address = 0
            };
            var regLux = new ModbusConfig
            {
                baudRate = 9600,
                device_address = 1,
                portName = "COM6",
                register_read_address = 1
            };

            var modbusProveder = new ModbusProvider();

            //add cancelation Token
            Console.WriteLine("start reading...");
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    var lux_mVoltage = modbusProveder.getValue<UInt16>(regLux);
                    lux = lux_mVoltage.ConvertToLux();
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine(modbusProveder.getValue<UInt16>(regCheck));
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("{0}      lux  from {1}    mV", lux, lux_mVoltage);
                    short bright = (short)((lux * max / 1000) + min);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("{0}  brightness set", bright);
                    //set brightness 0-1000 to 0-256
                    var result = Brightness.SetBrightness(bright);
                    if (!result)
                        //Brightness.SetBrightness(255);
                        //await Task.Delay(5000);
                        Console.WriteLine(result);
                }
                catch (Exception)
                {
                    Console.WriteLine("No connect");
                }
                //await Task.Delay(50);
            }
        }

        /// <summary>
        /// Convert  mV value to lux 
        /// </summary>
        /// <param name="m_volts"></param>
        /// <returns></returns>
        public static double ConvertToLux(this UInt16 m_volts)
        {
            double volts = (double)m_volts / 1000;
            double amps = volts / 10000.0;  // across 10,000 Ohms
            double microamps = amps * 1000000;
            double lux = microamps * 2.0;
            return lux;
        }
    }
}
