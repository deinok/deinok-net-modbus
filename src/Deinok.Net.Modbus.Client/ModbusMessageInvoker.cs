using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client
{

    public class ModbusMessageInvoker : IDisposable
    {

        private readonly ModbusMessageHandler modbusMessageHandler;
        private readonly bool disposeHandler;

        public ModbusMessageInvoker(ModbusMessageHandler modbusMessageHandler, bool disposeHandler = true)
        {
            this.modbusMessageHandler = modbusMessageHandler;
            this.disposeHandler = disposeHandler;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.disposeHandler)
            {
                this.modbusMessageHandler.Dispose();
            }
        }

        public ModbusMessage Send(ModbusMessage modbusMessage)
        {
            return this.modbusMessageHandler.Send(modbusMessage);
        }

        public async Task<ModbusMessage> SendAsync(ModbusMessage modbusMessage, CancellationToken cancellationToken = default)
        {
            return await this.modbusMessageHandler.SendAsync(modbusMessage, cancellationToken).ConfigureAwait(false);
        }

    }

}
