using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Linq;
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
            this.tcpClient = tcpClient;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tcpClient.Dispose();
            }
        }

        public override ModbusMessage Send(ModbusMessage modbusMessage)
        {
            var networkStream = this.tcpClient.GetStream();

            var writeBufferHeader = ArrayPool<byte>.Shared.Rent(6);
            var writeBufferHeaderSpan = writeBufferHeader.AsSpan(0, 6);
            var writeTransactionIdentifier = (ushort)Random.Shared.Next(ushort.MaxValue);
            var writeTransactionIdentifierSpan = writeBufferHeaderSpan.Slice(0, 2);
            BinaryPrimitives.WriteUInt16BigEndian(writeTransactionIdentifierSpan, writeTransactionIdentifier);
            var writeProtocolIdentifier = (ushort)0;
            var writeProtocolIdentifierSpan = writeBufferHeaderSpan.Slice(2, 2);
            BinaryPrimitives.WriteUInt16BigEndian(writeProtocolIdentifierSpan, writeProtocolIdentifier);
            var writeLength = (ushort)(2 + modbusMessage.Data.Length);
            var writeLengthSpan = writeBufferHeaderSpan.Slice(4, 2);
            BinaryPrimitives.WriteUInt16BigEndian(writeLengthSpan, writeLength);
            networkStream.Write(writeBufferHeaderSpan);
            var writeBufferBody = ArrayPool<byte>.Shared.Rent(2 + modbusMessage.Data.Length);
            var writeBufferBodySpan = writeBufferBody.AsSpan(0, 2 + modbusMessage.Data.Length);
            var writeAddress = modbusMessage.Address;
            var writeAddressSpan = writeBufferBodySpan.Slice(0, 1);
            writeAddressSpan[0] = writeAddress;
            var writeFunctionCode = modbusMessage.Function;
            var writeFunctionCodeSpan = writeBufferBodySpan.Slice(1, 1);
            writeFunctionCodeSpan[0] = (byte)writeFunctionCode;
            var writeData = modbusMessage.Data.ToArray();
            var writeDataSpan = writeBufferBodySpan.Slice(2, modbusMessage.Data.Length);
            writeData.CopyTo(writeDataSpan);
            networkStream.Write(writeBufferBodySpan);
            networkStream.Flush();
            ArrayPool<byte>.Shared.Return(writeBufferHeader);
            ArrayPool<byte>.Shared.Return(writeBufferBody);

            var readBufferHeader = ArrayPool<byte>.Shared.Rent(6);
            var readBufferHeaderSpan = writeBufferHeader.AsSpan(0, 6);
            networkStream.ReadExactly(readBufferHeaderSpan);
            var readTransactionIdentifierSpan = readBufferHeaderSpan.Slice(0, 2);
            var readTransactionIdentifier = BinaryPrimitives.ReadUInt16BigEndian(readTransactionIdentifierSpan);
            var readProtocolIdentifierSpan = readBufferHeaderSpan.Slice(2, 2);
            var readProtocolIdentifier = BinaryPrimitives.ReadUInt16BigEndian(readProtocolIdentifierSpan);
            var readLengthSpan = readBufferHeaderSpan.Slice(4, 2);
            var readLength = BinaryPrimitives.ReadUInt16BigEndian(readLengthSpan);
            var readBufferBody = ArrayPool<byte>.Shared.Rent(readLength);
            var readBufferBodySpan = readBufferBody.AsSpan(0, readLength);
            networkStream.ReadExactly(readBufferBodySpan);
            var readAdressSpan = readBufferBodySpan.Slice(0, 1);
            var readAddress = readAdressSpan[0];
            var readFunctionCodeSpan = readBufferBodySpan.Slice(1, 1);
            var readFunctionCode = readFunctionCodeSpan[0];
            var readDataSpan = readBufferBodySpan.Slice(2, readLength - 2);
            var readData = readDataSpan.ToArray();
            ArrayPool<byte>.Shared.Return(readBufferHeader);
            ArrayPool<byte>.Shared.Return(readBufferBody);

            return new ModbusMessage
            {
                Address = readAddress,
                Data = readData.ToImmutableArray(),
                Function = (ModbusFunction)readFunctionCode,
            };
        }

        public override async Task<ModbusMessage> SendAsync(ModbusMessage modbusMessage, CancellationToken cancellationToken = default)
        {
            var requestBuffer = ArrayPool<byte>.Shared.Rent(2 + 2 + 2 + 1 + 1 + modbusMessage.Data.Length);
            await Task.CompletedTask.ConfigureAwait(false);
            throw new NotImplementedException();
        }

    }

}
