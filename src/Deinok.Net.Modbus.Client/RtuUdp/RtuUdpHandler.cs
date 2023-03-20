using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client.RtuUdp
{

    public class RtuUdpHandler : ModbusMessageHandler
    {

        private readonly UdpClient udpClient;

        public RtuUdpHandler(UdpClient udpClient)
        {
            this.udpClient = udpClient;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.udpClient.Dispose();
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
