﻿using System;
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
            var networkStream = this.tcpClient.GetStream();

            var writeBufferHeader = ArrayPool<byte>.Shared.Rent(6);
            var writeBufferHeaderMemory = writeBufferHeader.AsMemory(0, 6);
            var writeTransactionIdentifier = (ushort)Random.Shared.Next(ushort.MaxValue);
            var writeTransactionIdentifierMemory = writeBufferHeaderMemory.Slice(0, 2);
            BinaryPrimitives.WriteUInt16BigEndian(writeTransactionIdentifierMemory.Span, writeTransactionIdentifier);
            var writeProtocolIdentifier = (ushort)0;
            var writeProtocolIdentifierMemory = writeBufferHeaderMemory.Slice(2, 2);
            BinaryPrimitives.WriteUInt16BigEndian(writeProtocolIdentifierMemory.Span, writeProtocolIdentifier);
            var writeLength = (ushort)(2 + modbusMessage.Data.Length);
            var writeLengthMemory = writeBufferHeaderMemory.Slice(4, 2);
            BinaryPrimitives.WriteUInt16BigEndian(writeLengthMemory.Span, writeLength);
            await networkStream.WriteAsync(writeBufferHeaderMemory, cancellationToken);
            var writeBufferBody = ArrayPool<byte>.Shared.Rent(2 + modbusMessage.Data.Length);
            var writeBufferBodyMemory = writeBufferBody.AsMemory(0, 2 + modbusMessage.Data.Length);
            var writeAddress = modbusMessage.Address;
            var writeAddressMemory = writeBufferBodyMemory.Slice(0, 1);
            writeAddressMemory.Span[0] = writeAddress;
            var writeFunctionCode = modbusMessage.Function;
            var writeFunctionCodeMemory = writeBufferBodyMemory.Slice(1, 1);
            writeFunctionCodeMemory.Span[0] = (byte)writeFunctionCode;
            var writeData = modbusMessage.Data.ToArray();
            var writeDataMemory = writeBufferBodyMemory.Slice(2, modbusMessage.Data.Length);
            writeData.CopyTo(writeDataMemory);
            await networkStream.WriteAsync(writeBufferBodyMemory, cancellationToken);
            await networkStream.FlushAsync(cancellationToken);
            ArrayPool<byte>.Shared.Return(writeBufferHeader);
            ArrayPool<byte>.Shared.Return(writeBufferBody);

            var readBufferHeader = ArrayPool<byte>.Shared.Rent(6);
            var readBufferHeaderMemory = writeBufferHeader.AsMemory(0, 6);
            await networkStream.ReadExactlyAsync(readBufferHeaderMemory, cancellationToken);
            var readTransactionIdentifierMemory = readBufferHeaderMemory.Slice(0, 2);
            var readTransactionIdentifier = BinaryPrimitives.ReadUInt16BigEndian(readTransactionIdentifierMemory.Span);
            var readProtocolIdentifierMemory = readBufferHeaderMemory.Slice(2, 2);
            var readProtocolIdentifier = BinaryPrimitives.ReadUInt16BigEndian(readProtocolIdentifierMemory.Span);
            var readLengthMemory = readBufferHeaderMemory.Slice(4, 2);
            var readLength = BinaryPrimitives.ReadUInt16BigEndian(readLengthMemory.Span);
            var readBufferBody = ArrayPool<byte>.Shared.Rent(readLength);
            var readBufferBodyMemory = readBufferBody.AsMemory(0, readLength);
            await networkStream.ReadExactlyAsync(readBufferBodyMemory, cancellationToken);
            var readAdressMemory = readBufferBodyMemory.Slice(0, 1);
            var readAddress = readAdressMemory.Span[0];
            var readFunctionCodeMemory = readBufferBodyMemory.Slice(1, 1);
            var readFunctionCode = readFunctionCodeMemory.Span[0];
            var readDataMemory = readBufferBodyMemory.Slice(2, readLength - 2);
            var readData = readDataMemory.ToArray();
            ArrayPool<byte>.Shared.Return(readBufferHeader);
            ArrayPool<byte>.Shared.Return(readBufferBody);

            return new ModbusMessage
            {
                Address = readAddress,
                Data = readData.ToImmutableArray(),
                Function = (ModbusFunction)readFunctionCode,
            };
        }

    }

}
