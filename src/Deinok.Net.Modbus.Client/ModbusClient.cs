using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client
{

    public class ModbusClient : ModbusMessageInvoker
    {

        public ModbusClient(ModbusMessageHandler modbusMessageHandler, bool disposeHandler = true) : base(modbusMessageHandler, disposeHandler)
        {
        }

        public async Task<ImmutableArray<ushort>> ReadMultipleHoldingRegistersAsync(byte address, ushort register, ushort length, CancellationToken cancellationToken = default)
        {
            using var writeMemoryOwner = MemoryPool<byte>.Shared.Rent(4);
            var writeMemory = writeMemoryOwner.Memory.Slice(0, 4);
            var registerMemory = writeMemory.Slice(0, 2);
            var lengthMemory = writeMemory.Slice(2, 2);
            BinaryPrimitives.WriteUInt16BigEndian(registerMemory.Span, register);
            BinaryPrimitives.WriteUInt16BigEndian(lengthMemory.Span, length);
            var modbusMessageRequest = new ModbusMessage
            {
                Address = address,
                Data = writeMemory.Span.ToImmutableArray(),
                Function = ModbusFunction.ReadMultipleHoldingRegisters,
            };
            var modbusMessageResponse = await this.SendAsync(modbusMessageRequest, cancellationToken);
            var dataMemory = modbusMessageResponse.Data.AsMemory();
            var dataBytesMemory = dataMemory.Slice(0, 1);
            var dataBytes = dataBytesMemory.Span[0];
            var dataContentMemory = dataMemory.Slice(1, dataBytes);
            var dataUshorts = dataBytes / 2;
            using var readMemoryOwner = MemoryPool<ushort>.Shared.Rent(dataUshorts);
            var readMemory = readMemoryOwner.Memory.Slice(0, dataUshorts);
            foreach (var index in Enumerable.Range(0, dataUshorts))
            {
                var readUshortMemory = dataContentMemory.Slice(index * 2, 2);
                readMemory.Span[index] = BinaryPrimitives.ReadUInt16BigEndian(readUshortMemory.Span);
            }
            return readMemory.Span.ToImmutableArray();
        }

    }

}
