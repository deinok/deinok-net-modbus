using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client.Udp
{

    public class UdpHandler : ModbusMessageHandler
    {

        private readonly UdpClient udpClient;

        public UdpHandler(UdpClient udpClient)
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
