using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client.Rtu
{

    public class RtuHandler : ModbusMessageHandler
    {

        private readonly SerialPort serialPort;

        public RtuHandler(SerialPort serialPort)
        {
            this.serialPort = serialPort;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.serialPort.Dispose();
            }
        }

        public override ModbusMessage Send(ModbusMessage modbusMessage)
        {
            throw new NotImplementedException();
        }

        public override async Task<ModbusMessage> SendAsync(ModbusMessage modbusMessage, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            throw new NotImplementedException();
        }

    }

}
