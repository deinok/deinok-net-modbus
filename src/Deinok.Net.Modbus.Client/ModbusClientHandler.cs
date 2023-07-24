using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client
{

    public class ModbusClientHandler : ModbusMessageHandler
    {

        public override async Task<ModbusMessage> SendAsync(ModbusMessage modbusMessage, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(modbusMessage, nameof(modbusMessage));
            await Task.CompletedTask.ConfigureAwait(false);
            throw new System.NotImplementedException();
        }

    }

}
