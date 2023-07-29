using System;
using System.Collections.Immutable;
using Xunit;

namespace Deinok.Net.Modbus.Tests
{

    public class ModbusMessageTests
    {

        [Fact]
        public void EqualityFact()
        {
            var modbusMessage1 = new ModbusMessage
            {
                Address = 1,
                Data = Array.Empty<byte>().ToImmutableArray(),
                Function = ModbusFunction.ReadCoils,
            };
            var modbusMessage2 = new ModbusMessage
            {
                Address = 1,
                Data = Array.Empty<byte>().ToImmutableArray(),
                Function = ModbusFunction.ReadCoils,
            };
            Assert.True(modbusMessage1 == modbusMessage2);
        }

    }

}