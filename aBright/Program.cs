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
        
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
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
                    
                    
                    var lux_mVoltage = modbusProveder.getValue<UInt16>(regLux);
                    lux = lux_mVoltage.ConvertToLux();
                    Console.Clear();
                    Console.WriteLine(modbusProveder.getValue<UInt16>(regCheck));
                    Console.WriteLine("{0} lux  from {1} mV",lux,lux_mVoltage);
                    
                }
                catch (Exception)
                {
                    Console.WriteLine("No connect");
                }

                await Task.Delay(500);
            }
        }


        public static double ConvertToLux(this UInt16 m_volts)
        {
            double volts = (double)m_volts /1000;
            double amps = volts / 10000.0;  // across 10,000 Ohms
            double microamps = amps * 1000000;
            double lux = microamps * 2.0;
            return lux;
        }
    }
}
