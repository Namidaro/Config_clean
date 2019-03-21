using System;
using System.Collections.Generic;
using ULA.Common.Formatters;

namespace ULA.Devices.PICON2.Business
{
    /// <summary>
    ///     Фабрика создания форматеров для Device data row
    /// </summary>
    public class FormatterFactory
    {
        /// <summary>
        ///     
        /// </summary>
        public FormatterFactory()
        {
            //var _formatters = new Dictionary<string, BinaryFormatterBase>
            //{
            //    {"ControlChannel.1NightLighting", new BytesToBooleanFormatter(1,0)},
            //    {"ControlChannel.1ControlMode", new BytesToBooleanFormatter(1,2)},
            //    {"ControlChannel.1Command",new BytesToBooleanFormatter(1,3)},
            //    {"ControlChannel.2NightLighting", new BytesToBooleanFormatter(1, 4)},
            //    {"ControlChannel.2ControlMode", new BytesToBooleanFormatter(1, 6)},
            //    {"ControlChannel.2Command", new BytesToBooleanFormatter(1, 7)},
            //    {"ControlChannel.3NightLighting", new BytesToBooleanFormatter(0, 0)},
            //    {"ControlChannel.3ControlMode", new BytesToBooleanFormatter(0, 2)},
            //    {"ControlChannel.3Command", new BytesToBooleanFormatter(0, 3)},
            //    {"State.Starter1",  new BytesToBooleanFormatter(8, 0)},
            //    {"State.Starter2",  new BytesToBooleanFormatter(8, 1)},
            //    {"State.Starter3",  new BytesToBooleanFormatter(8, 2)},
            //    {"Fuse.Number1", new BytesToBooleanFormatter(5, 0)},
            //    {"Fuse.Number2", new BytesToBooleanFormatter(5, 1)},
            //    {"Fuse.Number3", new BytesToBooleanFormatter(5, 2)},
            //    {"Fuse.Number4", new BytesToBooleanFormatter(5, 3)},
            //    {"Fuse.Number5", new BytesToBooleanFormatter(5, 4)},
            //    {"Fuse.Number6", new BytesToBooleanFormatter(5, 5)},
            //    {"Fuse.Number7", new BytesToBooleanFormatter(5, 6)},
            //    {"Fuse.Number8", new BytesToBooleanFormatter(5, 7)},
            //    {"Fuse.Number9", new BytesToBooleanFormatter(4, 0)},
            //    {"Fuse.Number10", new BytesToBooleanFormatter(4, 1)},
            //    {"Fuse.Number11", new BytesToBooleanFormatter(4, 2)},
            //    {"Fuse.Number12", new BytesToBooleanFormatter(4, 3)},
            //};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="starterNumber">Номер пускателя (нумерация с единицы)</param>
        /// <param name="chanelNumber">Номер канала. Нумерация с нуля</param>
        /// <returns>СЛоварь где ключи - это названия строк для данного пускателя, а значения - это форматеры этих строк в зависимости от канала</returns>
        public Dictionary<string, BinaryFormatterBase> GetFormattersForStarterByChanel(int starterNumber,
            int chanelNumber)
        {
            Dictionary<string, BinaryFormatterBase> formatters = null;

            var chanelMagicValue = chanelNumber*4;
            var index = 0;
            if (chanelNumber == 0 || chanelNumber == 1)
            {
                index = 0;
            }
            else if (chanelNumber == 2 || chanelNumber == 3)
            {
                index = -1;
            }
            if (chanelNumber == 4 || chanelNumber == 5)
            {
                index = 2;
            }
            if (chanelNumber == 6 || chanelNumber == 7)
            {
                index = 1;
            }

            if (starterNumber == 1)
            {
                formatters = new Dictionary<string, BinaryFormatterBase>
                {
                    {
                        "ControlChannel.1NightLighting",
                        new BytesToBooleanFormatter(1 + index, chanelMagicValue%8)
                    }
                    ,
                    {
                        "ControlChannel.1ControlMode",
                        new BytesToBooleanFormatter(1 + index, 2 + chanelMagicValue%8)
                    }
                    ,
                    {
                        "ControlChannel.1Command",
                        new BytesToBooleanFormatter(1 + index, 3 + chanelMagicValue%8)
                    }
                    ,

                    {
                        "State.Starter1",
                        new BytesToBooleanFormatter(8, chanelNumber)
                    }
                    ,

                    {
                        "Fuse.Number1",
                        new BytesToBooleanFormatter(5 + index, 0 + chanelMagicValue%8)
                    }
                    ,
                    {
                        "Fuse.Number2",
                        new BytesToBooleanFormatter(5 + index, 1 + chanelMagicValue%8)
                    }
                    ,
                    {
                        "Fuse.Number3",
                        new BytesToBooleanFormatter(5 + index, 2 + chanelMagicValue%8)
                    }
                    ,
                    {
                        "Fuse.Number4",
                        new BytesToBooleanFormatter(5 + index, 3 + chanelMagicValue%8)
                    }
                };


            }
            else if (starterNumber == 2)
            {
                formatters = new Dictionary<string, BinaryFormatterBase>
                {
                    {"ControlChannel.2NightLighting", new BytesToBooleanFormatter(1 + index, chanelMagicValue%8)}
                ,
                {
                    "ControlChannel.2ControlMode",
                    new BytesToBooleanFormatter(1 + index, 2+chanelMagicValue%8)
                }
            ,
                {
                    "ControlChannel.2Command",
                    new BytesToBooleanFormatter(1 + index, 3 + chanelMagicValue%8)
                }
            ,

                {
                    "State.Starter2",
                    new BytesToBooleanFormatter(8, chanelNumber)
                }
            ,

                {
                    "Fuse.Number5",
                    new BytesToBooleanFormatter(5+ index, 0+chanelMagicValue%8)
                }
            ,
                {
                    "Fuse.Number6",
                    new BytesToBooleanFormatter(5+ index, 1+chanelMagicValue%8)
                }
            ,
                {
                    "Fuse.Number7",
                    new BytesToBooleanFormatter(5+ index, 2+chanelMagicValue%8)
                }
            ,
                {
                    "Fuse.Number8",
                    new BytesToBooleanFormatter(5+ index, 3+chanelMagicValue%8)
                }
            }
            ;
            }
            else if (starterNumber == 3)
            {
                formatters = new Dictionary<string, BinaryFormatterBase>
                {
                    {"ControlChannel.3NightLighting", new BytesToBooleanFormatter(1 + index, chanelMagicValue%8)}
                ,
                {
                    "ControlChannel.3ControlMode",
                    new BytesToBooleanFormatter(1 + index, 2+chanelMagicValue%8)
                }
            ,
                {
                    "ControlChannel.3Command",
                    new BytesToBooleanFormatter(1 + index, 3 + chanelMagicValue%8)
                }
            ,

                {
                    "State.Starter3",
                    new BytesToBooleanFormatter(8, chanelNumber)
                }
            ,

                {
                    "Fuse.Number9",
                    new BytesToBooleanFormatter(5+ index, 0+chanelMagicValue%8)
                }
            ,
                {
                    "Fuse.Number10",
                    new BytesToBooleanFormatter(5+ index, 1+chanelMagicValue%8)
                }
            ,
                {
                    "Fuse.Number11",
                    new BytesToBooleanFormatter(5+ index, 2+chanelMagicValue%8)
                }
            ,
                {
                    "Fuse.Number12",
                    new BytesToBooleanFormatter(5+ index, 3+chanelMagicValue%8)
                }
            }
            ;
            }
            else
            {
                throw new ArgumentException("Номер стартера должен быть от 1 до 3. (из FormatterFactory)");
            }

            return formatters;
        }
    }
}
