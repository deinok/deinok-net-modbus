using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client.Ascii
{

    public class AsciiHandler : ModbusMessageHandler
    {

        private readonly SerialPort serialPort;

        public AsciiHandler(SerialPort serialPort)
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

        public override ModbusMessage Send(ModbusMessage request)
        {
            throw new NotImplementedException();
        }

        public override async Task<ModbusMessage> SendAsync(ModbusMessage request, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            throw new NotImplementedException();
        }

    }

}
