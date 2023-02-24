using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client
{

    public abstract class ModbusMessageHandler : IDisposable
    {

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) { }

        public abstract ModbusMessage Send(ModbusMessage request);

        public abstract Task<ModbusMessage> SendAsync(ModbusMessage request, CancellationToken cancellationToken = default);

    }

}
