using DevExpress.XtraPrinting.Shape.Native;
using NModbus;
using NModbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ModbusSlaveConfig
    {
        private TcpListener tcpListener;
        private IModbusFactory factory = new ModbusFactory();

        public Task Init()
        {
            tcpListener = new(IPAddress.Parse(Ip), Port);
            tcpListener.Start();
            IModbusFactory factory = new ModbusFactory();
            IModbusSlaveNetwork network = factory.CreateSlaveNetwork(tcpListener);
            IModbusSlave slave = factory.CreateSlave((byte)Station);
            network.AddSlave(slave);
            //network.
            var bgThread = new Thread(() =>
            {
                network.ListenAsync().GetAwaiter().GetResult();
            });
            bgThread.IsBackground = true;
            bgThread.Start();
            return Task.CompletedTask;
        }

        //private static void OnModbusSlaveRequestReceived(object sender, ModbusSlaveRequestEventArgs e)
        //{
        //    Console.WriteLine("Modbus request received: " + e.Message);
        //}

        //private static void OnDataStoreWrittenTo(object sender, DataStoreEventArgs e)
        //{
        //    Console.WriteLine("Data store written to: " + e.ModbusDataType);

        //}
    }
}
