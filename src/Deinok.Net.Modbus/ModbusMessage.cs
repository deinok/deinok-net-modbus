using System.Collections.Immutable;

namespace Deinok.Net.Modbus
{

    public record ModbusMessage
    {

        public required byte Address { get; init; }

        public required ImmutableArray<byte> Data { get; init; }

        public required ModbusFunction Function { get; init; }

    }

}
