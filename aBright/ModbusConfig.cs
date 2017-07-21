using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aBright.Abstract;

namespace aBright
{
    public class ModbusConfig : IModbusConfig
    {
        public string portName { get; set; }
        public int baudRate { get; set; }
        public byte device_address { get; set; }
        public ushort register_read_address { get; set; }
        public ushort register_write_address { get; set; }
        public RegType regType { get; set; }
    }
}
