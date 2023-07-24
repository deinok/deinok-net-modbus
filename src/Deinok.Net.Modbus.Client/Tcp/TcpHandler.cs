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
            {
                using var headerMemoryOwner = MemoryPool<byte>.Shared.Rent(6);
                var headerMemory = headerMemoryOwner.Memory.Slice(0, 6);
                var transactionIdentifierMemory = headerMemory.Slice(0, 2);
                var protocolIdentifierMemory = headerMemory.Slice(2, 2);
                var lengthMemory = headerMemory.Slice(4, 2);
                BinaryPrimitives.WriteUInt16BigEndian(transactionIdentifierMemory.Span, (ushort)Random.Shared.Next(ushort.MaxValue));
                BinaryPrimitives.WriteUInt16BigEndian(protocolIdentifierMemory.Span, 0);
                BinaryPrimitives.WriteUInt16BigEndian(lengthMemory.Span, (ushort)(2 + modbusMessage.Data.Length));
                await networkStream.WriteAsync(headerMemory, cancellationToken).ConfigureAwait(false);
                using var bodyMemoryOwner = MemoryPool<byte>.Shared.Rent(2 + modbusMessage.Data.Length);
                var bodyMemory = bodyMemoryOwner.Memory.Slice(0, 2 + modbusMessage.Data.Length);
                var addressMemory = bodyMemory.Slice(0, 1);
                var functionCodeMemory = bodyMemory.Slice(1, 1);
                var dataMemory = bodyMemory.Slice(2, modbusMessage.Data.Length);
                addressMemory.Span[0] = modbusMessage.Address;
                functionCodeMemory.Span[0] = (byte)modbusMessage.Function;
                modbusMessage.Data.CopyTo(dataMemory.Span);
                await networkStream.WriteAsync(bodyMemory, cancellationToken).ConfigureAwait(false);
                await networkStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            {
                using var headerMemoryOwner = MemoryPool<byte>.Shared.Rent(6);
                var headerMemory = headerMemoryOwner.Memory.Slice(0, 6);
                await networkStream.ReadExactlyAsync(headerMemory, cancellationToken).ConfigureAwait(false);
                var transactionIdentifierMemory = headerMemory.Slice(0, 2);
                var protocolIdentifierMemory = headerMemory.Slice(2, 2);
                var lengthMemory = headerMemory.Slice(4, 2);
                var transactionIdentifier = BinaryPrimitives.ReadUInt16BigEndian(transactionIdentifierMemory.Span);
                var protocolIdentifier = BinaryPrimitives.ReadUInt16BigEndian(protocolIdentifierMemory.Span);
                var length = BinaryPrimitives.ReadUInt16BigEndian(lengthMemory.Span);
                using var bodyMemoryOwner = MemoryPool<byte>.Shared.Rent(length);
                var bodyMemory = bodyMemoryOwner.Memory.Slice(0, length);
                await networkStream.ReadExactlyAsync(bodyMemory, cancellationToken).ConfigureAwait(false);
                var addressMemory = bodyMemory.Slice(0, 1);
                var functionCodeMemory = bodyMemory.Slice(1, 1);
                var dataMemory = bodyMemory.Slice(2, length - 2);
                var address = addressMemory.Span[0];
                var functionCode = functionCodeMemory.Span[0];
                var data = dataMemory.ToArray();
                return new ModbusMessage
                {
                    Address = address,
                    Data = data.ToImmutableArray(),
                    Function = (ModbusFunction)functionCode,
                };
            }
        }

    }

}
