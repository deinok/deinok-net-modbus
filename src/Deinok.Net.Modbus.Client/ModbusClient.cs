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

        public async Task<ImmutableArray<bool>> ReadCoilsAsync(byte address, ushort register, ushort length, CancellationToken cancellationToken = default)
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
                Function = ModbusFunction.ReadCoils,
            };
            var modbusMessageResponse = await this.SendAsync(modbusMessageRequest, cancellationToken).ConfigureAwait(false);
            var dataMemory = modbusMessageResponse.Data.AsMemory();
            var dataBytesMemory = dataMemory.Slice(0, 1);
            var dataBytes = dataBytesMemory.Span[0];
            var dataContentMemory = dataMemory.Slice(1, dataBytes);
            var dataBools = dataBytes * 8;
            using var readMemoryOwner = MemoryPool<bool>.Shared.Rent(dataBools);
            var readMemory = readMemoryOwner.Memory.Slice(0, dataBools);
            foreach (var index in Enumerable.Range(0, dataBytes))
            {
                var value = dataContentMemory.Span[index];
                readMemory.Span[0 + index * 8] = ((value >> 0) & 1) != 0;
                readMemory.Span[1 + index * 8] = ((value >> 1) & 1) != 0;
                readMemory.Span[2 + index * 8] = ((value >> 2) & 1) != 0;
                readMemory.Span[3 + index * 8] = ((value >> 3) & 1) != 0;
                readMemory.Span[4 + index * 8] = ((value >> 4) & 1) != 0;
                readMemory.Span[5 + index * 8] = ((value >> 5) & 1) != 0;
                readMemory.Span[6 + index * 8] = ((value >> 6) & 1) != 0;
                readMemory.Span[7 + index * 8] = ((value >> 7) & 1) != 0;
            }
            return readMemory.Slice(0, length).Span.ToImmutableArray();
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
            var modbusMessageResponse = await this.SendAsync(modbusMessageRequest, cancellationToken).ConfigureAwait(false);
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
