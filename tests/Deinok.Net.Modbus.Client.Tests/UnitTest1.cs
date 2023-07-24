using Deinok.Net.Modbus.Client.Tcp;
using System.Collections.Immutable;
using System.Net.Sockets;
using Xunit;

namespace Deinok.Net.Modbus.Client.Tests
{

    public class UnitTest1
    {

        [Fact(Skip = "Skip")]
        public async void Test1()
        {
            var tcpClient = new TcpClient("localhost", 502);
            var tcpHandler = new TcpHandler(tcpClient);
            var modbusMessageRequest = new ModbusMessage
            {
                Address = 1,
                Data = new byte[] { 0x00, 0x00, 0x00, 0x0A }.ToImmutableArray(),
                Function = ModbusFunction.ReadMultipleHoldingRegisters,
            };
            var modbusMessageResponse = await tcpHandler.SendAsync(modbusMessageRequest);
        }

    }

}