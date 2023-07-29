using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Deinok.Net.Modbus.Client.Tcp
{

    public class TcpHandler : ModbusMessageHandler
    {

        private readonly TcpClient tcpClient;

        public TcpHandler(TcpClient tcpClient)
        {
            ArgumentNullException.ThrowIfNull(tcpClient, nameof(tcpClient));
            this.tcpClient = tcpClient;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tcpClient.Dispose();
            }
        }

        public override async Task<ModbusMessage> SendAsync(ModbusMessage modbusMessage, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(modbusMessage, nameof(modbusMessage));
            var networkStream = this.tcpClient.GetStream();
            var writeTransactionIdentifier = (ushort)Random.Shared.Next(0, ushort.MaxValue);
            var writeProtocolIdentifier = (ushort)0;
            var writeLength = (ushort)(2 + modbusMessage.Data.Length);
            using var writeHeaderMemoryOwner = MemoryPool<byte>.Shared.Rent(6);
            var writeHeaderMemory = writeHeaderMemoryOwner.Memory[0..6];
            var writeTransactionIdentifierMemory = writeHeaderMemory[0..2];
            var writeProtocolIdentifierMemory = writeHeaderMemory[2..4];
            var writeLengthMemory = writeHeaderMemory[4..6];
            BinaryPrimitives.WriteUInt16BigEndian(writeTransactionIdentifierMemory.Span, writeTransactionIdentifier);
            BinaryPrimitives.WriteUInt16BigEndian(writeProtocolIdentifierMemory.Span, writeProtocolIdentifier);
            BinaryPrimitives.WriteUInt16BigEndian(writeLengthMemory.Span, writeLength);
            await networkStream.WriteAsync(writeHeaderMemory, cancellationToken).ConfigureAwait(false);
            using var writeBodyMemoryOwner = MemoryPool<byte>.Shared.Rent(writeLength);
            var writeBodyMemory = writeBodyMemoryOwner.Memory[0..writeLength];
            var writeAddressMemory = writeBodyMemory[0..1];
            var writeFunctionCodeMemory = writeBodyMemory[1..2];
            var writeDataMemory = writeBodyMemory[2..writeLength];
            writeAddressMemory.Span[0] = modbusMessage.Address;
            writeFunctionCodeMemory.Span[0] = (byte)modbusMessage.Function;
            modbusMessage.Data.CopyTo(writeDataMemory.Span);
            await networkStream.WriteAsync(writeBodyMemory, cancellationToken).ConfigureAwait(false);
            await networkStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            using var readHeaderMemoryOwner = MemoryPool<byte>.Shared.Rent(6);
            var readHeaderMemory = readHeaderMemoryOwner.Memory[0..6];
            await networkStream.ReadExactlyAsync(readHeaderMemory, cancellationToken).ConfigureAwait(false);
            var readTransactionIdentifierMemory = readHeaderMemory[0..2];
            var readProtocolIdentifierMemory = readHeaderMemory[2..4];
            var readLengthMemory = readHeaderMemory[4..6];
            var readTransactionIdentifier = BinaryPrimitives.ReadUInt16BigEndian(readTransactionIdentifierMemory.Span);
            var readProtocolIdentifier = BinaryPrimitives.ReadUInt16BigEndian(readProtocolIdentifierMemory.Span);
            var readLength = BinaryPrimitives.ReadUInt16BigEndian(readLengthMemory.Span);
            using var readBodyMemoryOwner = MemoryPool<byte>.Shared.Rent(readLength);
            var readBodyMemory = readBodyMemoryOwner.Memory[0..readLength];
            await networkStream.ReadExactlyAsync(readBodyMemory, cancellationToken).ConfigureAwait(false);
            var readAddressMemory = readBodyMemory[0..1];
            var readFunctionCodeMemory = readBodyMemory[1..2];
            var readDataMemory = readBodyMemory[2..readLength];
            var readAddress = readAddressMemory.Span[0];
            var readFunctionCode = readFunctionCodeMemory.Span[0];
            var readData = readDataMemory.Span.ToImmutableArray();
            if (writeTransactionIdentifier != readTransactionIdentifier)
            {
                throw new ModbusException($"Transaction Identifier is {readTransactionIdentifier} instead of {writeTransactionIdentifier}");
            }
            if (readProtocolIdentifier != 0)
            {
                throw new ModbusException($"Protocol Identifier is {readProtocolIdentifier} instead of {writeProtocolIdentifier}");
            }
            return new ModbusMessage
            {
                Address = readAddress,
                Data = readData,
                Function = (ModbusFunction)readFunctionCode,
            };
        }

    }

}
