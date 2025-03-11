using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public class ModbusSlaveWithLogging : IModbusSlave
    {
        private readonly IModbusSlave _innerSlave;

        public ModbusSlaveWithLogging(IModbusSlave innerSlave)
        {
            _innerSlave = innerSlave;
        }

        public byte UnitId => _innerSlave.UnitId;

        public ISlaveDataStore DataStore => _innerSlave.DataStore;

        public IModbusMessage ApplyRequest(IModbusMessage request)
        {
            // 在此處攔截請求並記錄相關信息
            Console.WriteLine($"[Request Received] Function Code: {request.FunctionCode}");
            Console.WriteLine($"Message Frame: {BitConverter.ToString(request.MessageFrame)}");

            // 繼續處理請求（呼叫內部的從站邏輯）
            return _innerSlave.ApplyRequest(request);

        }
    }
}
