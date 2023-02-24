using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client
{

    public class ModbusClientHandler : ModbusMessageHandler
    {

        public override ModbusMessage Send(ModbusMessage request)
        {
            throw new System.NotImplementedException();
        }

        public override async Task<ModbusMessage> SendAsync(ModbusMessage request, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            throw new System.NotImplementedException();
        }

    }

}
