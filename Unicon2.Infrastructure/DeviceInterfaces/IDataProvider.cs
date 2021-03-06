﻿using System;
using System.Threading.Tasks;
using Unicon2.Infrastructure.Connection;

namespace Unicon2.Infrastructure.DeviceInterfaces
{
    public interface IDataProvider : IDisposable
    {

        /// <summary>
        /// Функция 3 чтение значений из нескольких регистров хранения 
        /// </summary>
        /// <param name="startAddress">адрес начала</param>
        /// <param name="numberOfPoints">количество считываемых слов</param>
        /// <param name="dataTitle">подпись</param>
        /// <returns></returns>
        Task<ushort[]> ReadHoldingResgistersAsync(ushort startAddress, ushort numberOfPoints, string dataTitle);

        Task<bool> ReadCoilStatusAsync(ushort coilAddress, string dataTitle);


        Task<bool[]> ReadCoilStatusAsync(ushort coilAddress, string dataTitle, ushort numberOfPoints);

        Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] dataToWrite, string dataTitle);

        Task WriteSingleCoilAsync(ushort coilAddress, bool valueToWrite, string dataTitle);
        Task WriteSingleRegisterAsync(ushort registerAddress, ushort valueToWrite, string dataTitle);



        bool LastQuerySucceed { get; }
    }
}