using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aBright.Abstract;
using Modbus.Device;
using System.IO.Ports;
using System.Threading;

namespace aBright
{
    //Класс провайдера для опроса регистров. Устойчив к одновременному опросу порта
    public class ModbusProvider : IModbusProvider
    {
        private object locker = new object();
        private object _locker = new object();
        public T getValue<T>(IModbusConfig config)
        {
            lock (locker)
            {
                SerialPort port = new SerialPort(config.portName, config.baudRate);
                try
                {

                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                    lock (_locker)
                    {
                        port.Open();
                        IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
                        master.Transport.ReadTimeout = 200;
                        master.Transport.WriteTimeout = 200;

                        //Получение данных
                        ushort value = master.ReadHoldingRegisters(config.device_address, config.register_read_address, 1)[0];

                        port.Close();
                        port.Dispose();

                        switch (config.regType)
                        {
                            case RegType.INT16:
                                short signed_value = 0;
                                //Отрицательные значения отсчитываются от максимального значения в обратную сторону -1 = 65534
                                if (value >= 32768)
                                {
                                    signed_value = (short)((short)value - 65535);
                                }
                                return (T)Convert.ChangeType(value, typeof(T));
                            case RegType.UINT16:
                                return (T)Convert.ChangeType(value, typeof(T));
                        }
                        return (T)Convert.ChangeType(1, typeof(T));
                    }
                }
                catch (Exception ex)
                {
                    port.Dispose();
                    throw ex;
                }
            }
        }

        public void setValue<T>(IModbusConfig config, T value)
        {
            lock (locker)
            {
                var toWrite = (ushort)Convert.ChangeType(value, typeof(ushort));

                SerialPort port = new SerialPort(config.portName, config.baudRate);
                if (port.IsOpen)
                {
                    port.Close();
                }
                lock (_locker)
                {
                    port.Open();
                    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
                    master.Transport.ReadTimeout = 1000;
                    master.Transport.WriteTimeout = 1000;

                    //Запись
                    master.WriteMultipleRegisters(config.device_address, config.register_write_address, new ushort[] { toWrite });

                    port.Close();
                    Thread.Sleep(100);
                }
            }
        }

        public void setValue1<T>(IModbusConfig config, T value)
        {
            lock (locker)
            {
                var toWrite = (ushort)Convert.ChangeType(value, typeof(ushort));

                SerialPort port = new SerialPort(config.portName, config.baudRate);
                if (port.IsOpen)
                {
                    port.Close();
                }
                lock (_locker)
                {
                    port.Open();
                    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
                    master.Transport.ReadTimeout = 1000;
                    master.Transport.WriteTimeout = 1000;

                    //Запись
                    master.WriteSingleRegister(config.device_address, config.register_write_address, toWrite);

                    port.Close();
                    Thread.Sleep(100);
                }
            }
        }
    }
}
