using System;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client
{

    public class ModbusMessageInvoker : IAsyncDisposable, IDisposable
    {

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async ValueTask DisposeAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
            throw new NotImplementedException();
        }

    }

}
