using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client.RtuTcp
{

    public class RtuTcpHandler : ModbusMessageHandler
    {

        private readonly TcpClient tcpClient;

        public RtuTcpHandler(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tcpClient.Dispose();
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
