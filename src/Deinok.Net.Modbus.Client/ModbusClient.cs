namespace Deinok.Net.Modbus.Client
{

    public class ModbusClient : ModbusMessageInvoker
    {

        public ModbusClient(ModbusMessageHandler modbusMessageHandler, bool disposeHandler = true) : base(modbusMessageHandler, disposeHandler)
        {
        }

    }

}
