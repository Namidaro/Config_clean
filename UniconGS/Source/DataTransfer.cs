using System;
using System.Collections.Generic;
using System.Linq;

namespace UniconGS.Source
{
    public static class DataTransfer
    {
        private static Connector _connector;
        public static Queue<Query> QueryQueue { get; private set; }
        public static void InitConnector(Connector connector)
        {
            _connector = connector;
            QueryQueue = new Queue<Query>();
        }

        public static void UnInit()
        {
            if(_connector!=null)
                _connector.UnInit();
        }
        /// <summary>
        /// Метод по считыванию слов
        /// </summary>
        /// <param name="slot">Слот, хранящий данные</param>
        /// <returns></returns>
        public static ushort[] ReadWords(Slot slot)
        {
            lock (typeof(DataTransfer))
            {
                bool res = false;
                List<byte[]> querys = new List<byte[]>();
                List<byte[]> answers = new List<byte[]>();
                List<ushort[]> values = new List<ushort[]>();

                try
                {
                    if (_connector.Connect())
                    {
                        slot.Loaded = false;

                        if (slot.Size >= 120)
                        {
                            PrepareListReadWords(ref querys, ref answers, slot);
                        }
                        else
                        {
                            byte[] query = Modbus.CreateReadWordsArray(_connector.KNNumber, _connector.DeviceNumber, slot.Start, slot.Size);
                            querys.Add(query);
                            byte[] answer = _connector.ReadWrite(query);
                            if (answer[2] != /*5 +*/ slot.Size * 2)
                            {
                                throw new Exception("Считано неверное количество байт");
                            }
                            answers.Add(answer);
                        }

                        for (int i = 0; i < answers.Count; i++)
                        {
                            List<byte> mbVals = new List<byte>(answers[i][2]);
                            mbVals = answers[i].ToList().Skip(3).Take(answers[i][2]).ToList();                        
                            ushort[] value = new ushort[(mbVals.Count ) / 2];
                            for (int j = 0; j < mbVals.Count; j += 2)
                            {
                                value[(j) / 2] = Common.TOWORD(mbVals[j], mbVals[j+1]);
                            }
                            values.Add(value);
                        }

                        ushort[] result = new ushort[slot.Size];
                        int lastByteIndex = 0;
                        for (int i = 0; i < values.Count; i++)
                        {
                            Array.ConstrainedCopy(values[i], 0, result, lastByteIndex, values[i].Length);
                            lastByteIndex += values[i].Length;
                        }

                        slot.Value = result;
                        slot.Loaded = true;
                        res = true;
                    }
                }
                catch(Exception ex)
                {
                    res = false;
                    if (slot != null)
                    {
                        slot.Loaded = false;
                    }
                }
                finally
                {
                    //if (_connector.Disconnect() || !res)
                    //{
                    //    res = false;
                    //}
                }
                return res ? slot.Value : null;
            }
        }

        public static bool WriteWords(Slot slot)
        {
            lock (typeof(DataTransfer))
            {
                bool res = true;
                List<byte[]> querys = new List<byte[]>();
                List<byte[]> answers = new List<byte[]>();

                try
                {
                    if (_connector.Connect())
                    {
                        if (slot.Size >= 120)
                        {
                            PrepareListWriteWords(ref querys, ref answers, slot);
                        }
                        else
                        {
                            byte[] query = Modbus.CreateWriteWordsArray(_connector.KNNumber, _connector.DeviceNumber,
                                slot.Start, slot.Value);
                            querys.Add(query);
                            byte[] answer = _connector.ReadWrite(query);
                            if (answer.Length != 8)
                            {
                                throw new Exception("Считано неверное количество байт");
                            }
                            answers.Add(answer);
                        }
                    }
                    else
                    {
                        res = false;
                    }
                }
                catch (Exception)
                {
                    res = false;
                }
                finally
                {
                    //if (_connector.Disconnect() || !res)
                    //{
                    //    res = false;
                    //}
                }
                return res;
            }
        }

        public static bool WriteWord(Slot slot)
        {
            lock (typeof(DataTransfer))
            {
                bool res = true;
                try
                {
                    if (_connector.Connect())
                    {
                        byte[] query = Modbus.CreateWriteWordArray(_connector.KNNumber, _connector.DeviceNumber,
                            slot.Start, slot.Value[0]);
                        byte[] answer = _connector.ReadWrite(query);
                        //if (answer[2] != 4)
                        //{
                        //    throw new Exception("Считано неверное количество байт");
                        //}
                    }
                    else
                    {
                        res = false;
                    }
                }
                catch
                {
                    res = false;
                }
                finally
                {
                    if (_connector.Disconnect() || !res)
                    {
                        res = false;
                    }
                }
                return res;
            }
        }

        #region Чтение или запись более одного слова
        private static void PrepareListReadWords(ref List<byte[]> querys, ref List<byte[]> answers, Slot slot)
        {
            lock (typeof(DataTransfer))
            {
                int temp = 0;
                ushort oneSlotSize = 0x64;

                int queryCount = slot.Size / oneSlotSize;
                ushort lastQuerySize = (ushort)(slot.Size % oneSlotSize);
                ushort addressTmp = slot.Start;

                if (lastQuerySize != 0)
                {
                    temp = 1;
                }

                for (int i = 0; i < queryCount + temp; i++)
                {
                    if (temp == 0)
                    {
                        byte[] query = Modbus.CreateReadWordsArray(_connector.KNNumber, _connector.DeviceNumber,
                            addressTmp, oneSlotSize);
                        querys.Add(query);
                        addressTmp += oneSlotSize;
                    }
                    else
                    {
                        if (i == queryCount + temp - 2)
                        {
                            byte[] query = Modbus.CreateReadWordsArray(_connector.KNNumber, _connector.DeviceNumber,
                                addressTmp, oneSlotSize);
                            querys.Add(query);
                            addressTmp += oneSlotSize;
                        }
                        else if (i == queryCount + temp - 1)
                        {
                            byte[] query = Modbus.CreateReadWordsArray(_connector.KNNumber, _connector.DeviceNumber,
                                addressTmp, lastQuerySize);
                            querys.Add(query);
                            addressTmp += oneSlotSize;
                        }
                        else
                        {
                            byte[] query = Modbus.CreateReadWordsArray(_connector.KNNumber, _connector.DeviceNumber,
                                addressTmp, oneSlotSize);
                            querys.Add(query);
                            addressTmp += oneSlotSize;
                        }
                    }
                }
                for (int i = 0; i < querys.Count; i++)
                {
                    if (temp == 0)
                    {
                        byte[] answer = _connector.ReadWrite(querys[i]);
                        if (answer[2] != 5 + oneSlotSize * 2)
                        {
                            throw new Exception("Считано неверное количество байт");
                        }
                        answers.Add(answer);
                    }
                    else
                    {
                        if (i != queryCount + temp - 1)
                        {
                            byte[] answer = _connector.ReadWrite(querys[i]);
                            if (answer[2] != oneSlotSize * 2)
                            {
                                throw new Exception("Считано неверное количество байт");
                            }
                            answers.Add(answer);
                        }
                        else
                        {
                            byte[] answer = _connector.ReadWrite(querys[i]);
                            if (answer[2] !=  lastQuerySize * 2)
                            {
                                throw new Exception("Считано неверное количество байт");
                            }
                            answers.Add(answer);
                        }
                    }
                }
            }
        }

        private static void PrepareListWriteWords(ref List<byte[]> querys, ref List<byte[]> answers, Slot slot)
        {
            lock (typeof(DataTransfer))
            {
                int temp = 0;
                ushort oneSlotSize = 0x64;
                List<ushort[]> values = new List<ushort[]>();

                // мы не можем записывать в устройство больше 256 байт.
                // поэтому большой запрос мы разбиваем на несколько
                int queryCount = slot.Size / oneSlotSize;
                ushort lastQuerySize = (ushort)(slot.Size % oneSlotSize);
                ushort addressTmp = slot.Start;

                if (lastQuerySize != 0)
                {
                    temp = 1;
                }

                // заполнение раздробленных частей
                int lastByteIndex = 0;
                for (int i = 0; i < queryCount + temp; i++)
                {
                    ushort[] value = null;
                    if (i == queryCount + temp - 1 && temp == 1)
                    {
                        value = new ushort[lastQuerySize];
                        Array.ConstrainedCopy((slot.Value), lastByteIndex, value, 0, value.Length);
                        lastByteIndex += value.Length;
                    }
                    else
                    {
                        value = new ushort[oneSlotSize];
                        Array.ConstrainedCopy((slot.Value), lastByteIndex, value, 0, value.Length);
                        lastByteIndex += value.Length;
                    }
                    values.Add(value);
                }

                // создание модбас запросов для каждой части
                for (int i = 0; i < queryCount + temp; i++)
                {
                    if (i == queryCount + temp - 2 && temp == 1)
                    {
                        byte[] query = Modbus.CreateWriteWordsArray(_connector.KNNumber, _connector.DeviceNumber,
                            addressTmp, values[i]);
                        querys.Add(query);
                        addressTmp += oneSlotSize;
                    }
                    else if (i == queryCount + temp - 1 && temp == 1)
                    {
                        byte[] query = Modbus.CreateWriteWordsArray(_connector.KNNumber, _connector.DeviceNumber,
                            addressTmp, values[i]);
                        querys.Add(query);
                        addressTmp += oneSlotSize;
                    }
                    else
                    {
                        byte[] query = Modbus.CreateWriteWordsArray(_connector.KNNumber, _connector.DeviceNumber,
                            addressTmp, values[i]);
                        querys.Add(query);
                        addressTmp += oneSlotSize;
                    }

                }

                //TODO: отправка запроса в устройство
                foreach (byte[] q in querys)
                {
                    byte[] answer = _connector.ReadWrite(q);

                    // проверить ответ
                    answers.Add(answer);
                }
            }
        }
        #endregion

        #region Доп. функции
        /// <summary>
        /// Ставит запрос в начало очереди
        /// </summary>
        /// <param name="control">Контрол, который идет в очередь</param>
        /// <param name="option">Опция на чтение или запись</param>
        /// <param name="isCycle">Цикличность запроса</param>
        public static void SetTopInQueue(IQuery control, Accsess option, bool isCycle)
        {
            Queue<Query> queue = new Queue<Query>(QueryQueue);
            QueryQueue.Clear();
            QueryQueue.Enqueue(new Query(control, isCycle, option));
            foreach (var q in queue)
            {
                QueryQueue.Enqueue(q);
            }
        }

        public static bool WriteBit(bool value, ushort addrbit)
        {
            var res = false;
            try
            {
                if (_connector.Connect())
                {
                    var queryKn = Modbus.CreateWriteBitArray(_connector.KNNumber, _connector.DeviceNumber, addrbit, value);
                    var answerKn = _connector.ReadWrite(queryKn);

                    var query = queryKn.Skip(2).Take(8).ToArray();
                    var answer = answerKn.Skip(2).Take(8).ToArray();
                    if (answer.Length == query.Length)
                    {
                        res = !query.Where((t, i) => t != answer[i]).Any();
                    }
                }
            }
            catch (Exception error)
            {
                return false;
            }
            finally
            {
                if (_connector.Disconnect())
                {
                    res = false;
                }
            }
            return res;
        }
        #endregion
    }
}
