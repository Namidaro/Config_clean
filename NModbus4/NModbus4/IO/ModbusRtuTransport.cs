using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NModbus4.Message;
using NModbus4.Utility;

namespace NModbus4.IO
{
    /// <summary>
    ///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class ModbusRtuTransport : ModbusSerialTransport
    {
        public const int RequestFrameStartLength = 7;

        public const int ResponseFrameStartLengthShort = 4;
        public const int ResponseFrameStartLengthLong = 6;

        internal ModbusRtuTransport(IStreamResource streamResource)
            : base(streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        public static int RequestBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];
            int numBytes;

            switch (functionCode)
            {
                case Modbus.ReadCoils:
                case Modbus.ReadInputs:
                case Modbus.ReadHoldingRegisters:
                case Modbus.ReadInputRegisters:
                case Modbus.WriteSingleCoil:
                case Modbus.WriteSingleRegister:
                case Modbus.Diagnostics:
                    numBytes = 1;
                    break;
                case Modbus.WriteMultipleCoils:
                case Modbus.WriteMultipleRegisters:
                    byte byteCount = frameStart[6];
                    numBytes = byteCount + 2;
                    break;
                default:
                    string msg = $"Function code {functionCode} not supported.";
                    Debug.WriteLine(msg);
                    throw new NotImplementedException(msg);
            }

            return numBytes;
        }

        public static int ResponseBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            // exception response
            if (functionCode > Modbus.ExceptionOffset)
            {
                return 1;
            }

            int numBytes;
            switch (functionCode)
            {
                case Modbus.ReadCoils:
                case Modbus.ReadInputs:
                case Modbus.ReadHoldingRegisters:
                case Modbus.ReadInputRegisters:
                    numBytes = frameStart[2] + 1;
                    break;
                case Modbus.WriteSingleCoil:
                case Modbus.WriteSingleRegister:
                case Modbus.WriteMultipleCoils:
                case Modbus.WriteMultipleRegisters:
                case Modbus.Diagnostics:
                    numBytes = 4;
                    break;
                case Modbus.Function12:
                    numBytes = frameStart[2];
                    break;
                case Modbus.RetranslateFunction:
                    switch(frameStart[4])
                    {
                        case Modbus.WriteSingleCoil:
                        case Modbus.WriteSingleRegister:
                        case Modbus.WriteMultipleCoils:
                        case Modbus.WriteMultipleRegisters:
                        case Modbus.Diagnostics:
                            numBytes = 7;
                            break;
                        default:
                            numBytes = frameStart[5];
                            break;
                    }
                    break;

                default:
                    string msg = $"Function code {functionCode} not supported.";
                    Debug.WriteLine(msg);
                    throw new NotImplementedException(msg);
            }

            return numBytes;
        }

        public virtual byte[] Read(int count)
        {
            byte[] frameBytes = new byte[count];
            int numBytesRead = 0;

            while (numBytesRead != count)
            {
                numBytesRead += StreamResource.Read(frameBytes, numBytesRead, count - numBytesRead);
            }

            return frameBytes;
        }

        internal override byte[] BuildMessageFrame(IModbusMessage message)
        {
            var messageFrame = message.MessageFrame;
            var crc = ModbusUtility.CalculateCrc(messageFrame);

            if (Modbus.KNNumber != 0)
            {
                var messageBody = new MemoryStream(messageFrame.Length + crc.Length + 4);
                messageBody.Write(new byte[] { Modbus.KNNumber }, 0, 1);
                messageBody.Write(new byte[] { 17 }, 0, 1);
                messageBody.Write(messageFrame, 0, messageFrame.Length);
                messageBody.Write(crc, 0, crc.Length);
                var crc2 = ModbusUtility.CalculateCrc(messageBody.ToArray());
                messageBody.Write(crc2, 0, crc2.Length);
                return messageBody.ToArray();
            }
            else
            {
                var messageBody = new MemoryStream(messageFrame.Length + crc.Length);
                messageBody.Write(messageFrame, 0, messageFrame.Length);
                messageBody.Write(crc, 0, crc.Length);
                return messageBody.ToArray();
            }

        }

        internal override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            return BitConverter.ToUInt16(messageFrame, messageFrame.Length - 2) ==
                BitConverter.ToUInt16(ModbusUtility.CalculateCrc(message.MessageFrame), 0);
        }

        internal override IModbusMessage ReadResponse<T>()
        {
            byte[] frameStart;
            if (Modbus.KNNumber != 0)
            {
                frameStart = Read(ResponseFrameStartLengthLong);
            }
            else
            {
                frameStart = Read(ResponseFrameStartLengthShort);
            }
            byte[] frameEnd = Read(ResponseBytesToRead(frameStart));
            byte[] frame = Enumerable.Concat(frameStart, frameEnd).ToArray();
            Debug.WriteLine($"RX: {string.Join(", ", frame)}");

            return CreateResponse<T>(frame);
        }

        internal override byte[] ReadRequest()
        {
            byte[] frameStart = Read(RequestFrameStartLength);
            byte[] frameEnd = Read(RequestBytesToRead(frameStart));
            byte[] frame = Enumerable.Concat(frameStart, frameEnd).ToArray();
            Debug.WriteLine($"RX: {string.Join(", ", frame)}");

            return frame;
        }
    }
}
