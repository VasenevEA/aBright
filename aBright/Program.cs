using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aBright
{
    static class Program
    {
        private static string _portName;

      
        [STAThread]
        static void Main()
        {
            _portName = portFinder();

            if (!String.IsNullOrEmpty(_portName))
                Task.Factory.StartNew(updater);
            else
                Console.WriteLine("Port Not finded");

            Console.ReadKey();
        }

        private static async void updater()
        {
            var regLux = new ModbusConfig
            {
                baudRate = 9600,
                device_address = 1,
                portName = _portName,
                register_read_address = 1
            };

            var modbusProveder = new ModbusProvider();

            //TODO: add cancelation Token
            while (true)
            {
                try
                {
                    var lux_mVoltage = modbusProveder.getValue<UInt16>(regLux);
                    var lux = lux_mVoltage.ConvertToLux();

                    short bright = (short)Math.Pow(lux, 2);
                    var result = Brightness.SetBrightness(bright);

                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("{0}  lux  from {1} mV     ", lux, lux_mVoltage);
                    Console.WriteLine("{0}  brightness set", bright);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No connect " + ex.Message);
                    await Task.Delay(1000);
                    Console.Clear();
                }
                await Task.Delay(100);
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

        /// <summary>
        /// Return portName with arduino Modbus slave
        /// </summary>
        /// <returns></returns>
        private static string portFinder()
        {
            var modbusProveder = new ModbusProvider();
            foreach (var __portName in SerialPort.GetPortNames())
            {
                ///Create config for check register
                ///reg 0 always return value = 200 
                var regCheck = new ModbusConfig
                {
                    baudRate = 9600,
                    device_address = 1,
                    portName = __portName,
                    register_read_address = 0
                };

                try
                {
                    if (modbusProveder.getValue<UInt16>(regCheck) == 200)
                        return __portName;
                }
                catch (Exception)
                {
                    //Bad way
                }
            }
            return null;
        }
    }
}
