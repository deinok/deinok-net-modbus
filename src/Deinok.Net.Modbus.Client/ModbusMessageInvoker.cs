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
            ArgumentNullException.ThrowIfNull(modbusMessageHandler, nameof(modbusMessageHandler));
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
            ArgumentNullException.ThrowIfNull(modbusMessage, nameof(modbusMessage));
            return this.modbusMessageHandler.Send(modbusMessage);
        }

        public async Task<ModbusMessage> SendAsync(ModbusMessage modbusMessage, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(modbusMessage, nameof(modbusMessage));
            return await this.modbusMessageHandler.SendAsync(modbusMessage, cancellationToken).ConfigureAwait(false);
        }

    }

}
