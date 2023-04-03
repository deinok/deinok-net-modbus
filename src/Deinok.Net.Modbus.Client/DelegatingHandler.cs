using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client
{

    public abstract class DelegatingHandler : ModbusMessageHandler
    {

        public ModbusMessageHandler InnerHandler { get; }

        protected DelegatingHandler(ModbusMessageHandler innerHandler)
        {
            ArgumentNullException.ThrowIfNull(innerHandler, nameof(innerHandler));
            this.InnerHandler = innerHandler;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.InnerHandler.Dispose();
            }
            base.Dispose(disposing);
        }

        public override ModbusMessage Send(ModbusMessage modbusMessage)
        {
            ArgumentNullException.ThrowIfNull(modbusMessage, nameof(modbusMessage));
            return this.InnerHandler.Send(modbusMessage);
        }

        public override async Task<ModbusMessage> SendAsync(ModbusMessage modbusMessage, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(modbusMessage, nameof(modbusMessage));
            return await this.InnerHandler.SendAsync(modbusMessage, cancellationToken).ConfigureAwait(false);
        }

    }

}
