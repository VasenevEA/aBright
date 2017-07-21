using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aBright.Abstract
{
    public interface IModbusConfig
    {
        string portName { get; set; }                    //Имя порта
        int baudRate { get; set; }                       //Скорость порта

        byte device_address { get; set; }                //Адрес устройства
        ushort register_read_address { get; set; }       //Адрес регистра
        ushort register_write_address { get; set; }      //Адрес регистра
        RegType regType { get; set; }                
    }

    public enum RegType
    {
        UINT16,
        INT16
    }
}
