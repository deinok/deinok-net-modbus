﻿using System;
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
            ArgumentNullException.ThrowIfNull(udpClient, nameof(udpClient));
            this.udpClient = udpClient;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.udpClient.Dispose();
            }
        }

        public override async Task<ModbusMessage> SendAsync(ModbusMessage modbusMessage, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(modbusMessage, nameof(modbusMessage));
            await Task.CompletedTask.ConfigureAwait(false);
            throw new NotImplementedException();
        }

    }

}
