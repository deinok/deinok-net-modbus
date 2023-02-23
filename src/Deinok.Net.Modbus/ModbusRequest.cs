using System.Collections.Immutable;

namespace Deinok.Net.Modbus
{

    public record ModbusRequest
    {

        public required byte Address { get; init; }

        public required ImmutableArray<byte> Data { get; init; }

        public required byte Function { get; init; }

    }

}
