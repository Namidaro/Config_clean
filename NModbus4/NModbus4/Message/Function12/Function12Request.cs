using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NModbus4.Message;
using NModBus4.Extensions;

namespace NModBus4.Message.Function12
{
    public class Function12Request : AbstractModbusMessage, IModbusRequest
    {

        private int _minimumFrameSize = 0;


        public Function12Request(byte functionCode, byte slaveAddress, byte moduleNumber, byte queryId, byte numberOfBytes) : base(slaveAddress, functionCode)
        {
            List<byte> customBytes = new List<byte>();
            customBytes.Add(moduleNumber);
            customBytes.Add(queryId);
            customBytes.Add(numberOfBytes);

            MessageImpl.CustomBytesInRequest = customBytes.ToArray();
        }




        #region Overrides of AbstractModbusMessage

        public override int MinimumFrameSize
        {
            get { return _minimumFrameSize; }
        }

        protected override void InitializeUnique(byte[] frame)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IModbusRequest

        public void ValidateResponse(IModbusMessage response)
        {
            //
        }

        #endregion
    }
}
