using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.Resources
{
    public class ModuleRequest : IModuleRequest
    {
        #region [Properties]
        public byte Period { get; set; }
        public byte Type { get; set; }
        public byte CrateAddress { get; set; }
        public byte Command { get; set; }
        public byte[] ParameterModuleAddress { get; set; }
        public byte[] ParameterBaseAddress { get; set; }
        public byte ParameterCount { get; set; }
        public ushort[] Request { get; set; }
        public ushort[] RequestToDevice { get; set; }
        public string UIRequest { get; set; }
        #endregion

        #region [Ctor's]
        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public ModuleRequest()
        {
            Period = 0x00;
            Type = 0x00;
            CrateAddress = 0x00;
            Command = 0x00;
            ParameterModuleAddress = new byte[2] { 0x00, 0x00 };
            ParameterBaseAddress = new byte[2] { 0x00, 0x00 };
            ParameterCount = 0x00;
            UIRequest = String.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_period"></param>
        /// <param name="_type"></param>
        /// <param name="_crateAddress"></param>
        /// <param name="_command"></param>
        /// <param name="_paramModAddr"></param>
        /// <param name="_paramBaseAddr"></param>
        /// <param name="_paramCount"></param>
        public ModuleRequest(byte _period,
                            byte _type,
                            byte _crateAddress,
                            byte _command,
                            byte[] _paramModAddr,
                            byte[] _paramBaseAddr,
                            byte _paramCount)
        {
            Period = _period;
            Type = _type;
            CrateAddress = _crateAddress;
            Command = _command;
            ParameterModuleAddress = _paramModAddr;
            ParameterBaseAddress = _paramBaseAddr;
            ParameterCount = _paramCount;

            CreateRequest();
            GenerateUIRequestAsString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_period"></param>
        /// <param name="_type"></param>
        /// <param name="_crateAddress"></param>
        /// <param name="_command"></param>
        /// <param name="_paramModAddr"></param>
        /// <param name="_paramBaseAddr"></param>
        /// <param name="_paramCount"></param>
        public ModuleRequest(byte _period,
                           byte _type,
                           byte _crateAddress,
                           byte _command,
                           ushort _paramModAddr,
                           ushort _paramBaseAddr,
                           byte _paramCount)
        {
            Period = _period;
            Type = _type;
            CrateAddress = _crateAddress;
            Command = _command;
            ParameterModuleAddress = new byte[2] { (byte)(_paramModAddr >> 8), (byte)(_paramModAddr & 0xff) };
            ParameterBaseAddress = new byte[2] { (byte)(_paramBaseAddr >> 8), (byte)(_paramBaseAddr & 0xff) };
            ParameterCount = _paramCount;

            CreateRequest();
            GenerateUIRequestAsString();
        }
        /// <summary>
        /// Конструктор для ответа из устройства
        /// </summary>
        /// <param name="req"></param>
        public ModuleRequest(ushort[] req)
        {
            Request = req;
            byte[] reqArray = ArrayExtension.UshortArrayToByteArray(Request);

            SpreadRequest(reqArray);
            GenerateUIRequestAsString();
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Создание запроса из свойств экземпляра
        /// </summary>
        public void CreateRequest()
        {
            try
            {
                List<byte> req = new List<byte>();
                req.Add(Period);
                req.Add((byte)((Type << 4) + CrateAddress));
                req.Add(Command);
                req.AddRange(ParameterModuleAddress);
                req.AddRange(ParameterBaseAddress);
                req.Add(ParameterCount);
                Request = ArrayExtension.ByteArrayToUshortArray(req.ToArray());

                byte[] reqToDev = req.ToArray();
                ArrayExtension.SwapArrayItems(ref reqToDev);
                RequestToDevice = ArrayExtension.ByteArrayToUshortArray(reqToDev);
            }
            catch { }
        }
        /// <summary>
        /// Заполнение свойств экземпляра по данным из запроса
        /// </summary>
        /// <param name="req"></param>
        public void SpreadRequest(byte[] req)
        {
            try
            {
                Period = 0x00;
                Type = 0x00;
                CrateAddress = 0x00;
                Command = 0x00;
                ParameterModuleAddress = new byte[2] { 0x00, 0x00 };
                ParameterBaseAddress = new byte[2] { 0x00, 0x00 };
                ParameterCount = 0x00;
                ArrayExtension.SwapArrayItems(ref req);
                if (req.Count() == 8)
                {
                    Period = Converters.Convert.ConvertFromDecToHex(req[0]);
                    Type = Converters.Convert.HIHALFBYTE(Converters.Convert.ConvertFromDecToHex(req[1]));
                    CrateAddress = Converters.Convert.LOHALFBYTE(Converters.Convert.ConvertFromDecToHex(req[1]));
                    Command = Converters.Convert.ConvertFromDecToHex(req[2]);
                    ParameterModuleAddress[0] = Converters.Convert.ConvertFromDecToHex(req[3]);
                    ParameterModuleAddress[1] = Converters.Convert.ConvertFromDecToHex(req[4]);
                    ParameterBaseAddress[0] = Converters.Convert.ConvertFromDecToHex(req[5]);
                    ParameterBaseAddress[1] = Converters.Convert.ConvertFromDecToHex(req[6]);
                    ParameterCount = Converters.Convert.ConvertFromDecToHex(req[7]);
                }
            }
            catch { }
        }
        /// <summary>
        /// Создание строки "запроса к модулю" (string),
        /// </summary>
        private void GenerateUIRequestAsString()
        {
            //TODO: не нравится, надо как-то переделать
            StringBuilder sb = new StringBuilder();
            if (Period < 10)
            {
                sb.Append("0" + Converters.Convert.ConvertFromDecToHexStr(Period) + "  ");
            }
            else
            {
                sb.Append(Converters.Convert.ConvertFromDecToHexStr(Period) + "  ");
            }
            sb.Append(Converters.Convert.ConvertFromDecToHexStr(Type));
            sb.Append(Converters.Convert.ConvertFromDecToHexStr(CrateAddress) + "  ");
            if (Command < 10)
            {
                sb.Append("0" + Converters.Convert.ConvertFromDecToHexStr(Command) + "  ");
            }
            else
            {
                sb.Append(Converters.Convert.ConvertFromDecToHexStr(Command) + "  ");
            }
            if (ParameterModuleAddress[0] < 10)
            {
                sb.Append("0" + Converters.Convert.ConvertFromDecToHexStr(ParameterModuleAddress[0]));
            }
            else
            {
                sb.Append(Converters.Convert.ConvertFromDecToHexStr(ParameterModuleAddress[0]));
            }
            if (ParameterModuleAddress[1] < 10)
            {
                sb.Append("0" + Converters.Convert.ConvertFromDecToHexStr(ParameterModuleAddress[1]) + "  ");
            }
            else
            {
                sb.Append(Converters.Convert.ConvertFromDecToHexStr(ParameterModuleAddress[1]) + "  ");
            }
            if (ParameterBaseAddress[0] < 10)
            {
                sb.Append("0" + Converters.Convert.ConvertFromDecToHexStr(ParameterBaseAddress[0]));
            }
            else
            {
                sb.Append(Converters.Convert.ConvertFromDecToHexStr(ParameterBaseAddress[0]));
            }
            if (Converters.Convert.ConvertFromDecToHexStr(ParameterBaseAddress[1]).Length==1)
            {
                sb.Append("0" + Converters.Convert.ConvertFromDecToHexStr(ParameterBaseAddress[1]) + "  ");
            }
            else
            {
                sb.Append(Converters.Convert.ConvertFromDecToHexStr(ParameterBaseAddress[1]) + "  ");
            }
            if (ParameterCount < 10)
            {
                sb.Append("0" + Converters.Convert.ConvertFromDecToHexStr(ParameterCount));
            }
            else if(Converters.Convert.ConvertFromDecToHexStr(ParameterCount).Length == 1)
            {
                sb.Append("0" + Converters.Convert.ConvertFromDecToHexStr(ParameterCount));
            }
            else
            {
                sb.Append(Converters.Convert.ConvertFromDecToHexStr(ParameterCount));
            }
            UIRequest = sb.ToString();
        }
        #endregion
    }
}