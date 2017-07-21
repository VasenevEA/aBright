using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aBright.Abstract
{
    public interface IModbusProvider
    {
        /// <summary>
        /// Возвращает значение переменной в соответствии с конфигом
        /// </summary>
        /// <typeparam name="T">тип конечной величины</typeparam>
        T getValue<T>(IModbusConfig config);

        /// <summary>
        /// Запись значения конкретного типа
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="value"></param>
        void setValue<T>(IModbusConfig config, T value);
    }
}
