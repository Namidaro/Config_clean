﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniconGS.Enums;

namespace UniconGS.UI.Configuration
{
    public static class ComboBoxItemStr
    {
        public static List<string> DataKU
        {
            get
            {
                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICONGS_LIDA_4DISCRET || DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICONGS_LIDA_2DISCRET)
                {
                    return new List<string>
                    {
                        "Нет",
                    "График освещения",
                    "График подсветки",
                    "График иллюминации",
                    "График обогрева",
                    "График освещения + экономия",
                    "График подсветки + экономия",
                    "График иллюминации + экономия"
                    };
                }
                else
                    return new List<string>
                {
                    "Нет",
                    "График освещения",
                    "График подсветки",
                    "График иллюминации",
                    "График энергосбережения",
                    "График обогрева",
                    "График освещения + экономия",
                    "График подсветки + экономия",
                    "График иллюминации + экономия"
                };
            }
        }

        public static List<string> OutputKU
        {
            get
            {
                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Реле №1",
                        "Реле №2",
                        "Реле №3"
                        //"Реле №4",
                        //"Реле №5",
                        //"Реле №6",
                        //   "Реле №7",
                        //"Реле №8"
                    };
                }

                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Реле №1",
                        "Реле №2",
                        "Реле №3",
                        "Реле №4",
                        "Реле №5",
                        "Реле №6",
                        "Реле №7",
                        "Реле №8"
                    };
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICONGS_LIDA_4DISCRET)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Реле №1",
                        "Реле №2",
                        "Реле №3",
                        "Реле №4",
                        "Реле №5",
                        "Реле №6",
                        "Реле №7",
                        "Реле №8"
                    };
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICONGS_LIDA_2DISCRET)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Реле №1",
                        "Реле №2",
                        "Реле №3",
                        "Реле №4",
                        "Реле №5",
                        "Реле №6",
                        "Реле №7",
                        "Реле №8"
                    };
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Реле №1",
                        "Реле №2",
                        "Реле №3",
                        "Реле №4",
                        "Реле №5",
                        "Реле №6",
                        "Реле №7",
                        "Реле №8"
                    };
                }


                else
                {
                    return new List<string>();
                };
            }
        }

        public static List<string> Control
        {
            get
            {
                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Модуль №1, дискрет №1",
                        "Модуль №1, дискрет №2",
                        "Модуль №1, дискрет №3",
                        "Модуль №1, дискрет №4",
                        "Модуль №1, дискрет №5",
                        "Модуль №1, дискрет №6",
                        "Модуль №1, дискрет №7",
                        "Модуль №1, дискрет №8",
                        "Модуль №1, дискрет №9",
                        "Модуль №1, дискрет №10",
                        "Модуль №1, дискрет №11"
                    };
                }


                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Модуль №1, дискрет №1",
                        "Модуль №1, дискрет №2",
                        "Модуль №1, дискрет №3",
                        "Модуль №1, дискрет №4",
                        "Модуль №1, дискрет №5",
                        "Модуль №1, дискрет №6",
                        "Модуль №1, дискрет №7",
                        "Модуль №1, дискрет №8",
                        "Модуль №1, дискрет №9",
                        "Модуль №1, дискрет №10",
                        "Модуль №1, дискрет №11",
                        "Модуль №2, дискрет №1",
                        "Модуль №2, дискрет №2",
                        "Модуль №2, дискрет №3",
                        "Модуль №2, дискрет №4",
                        "Модуль №2, дискрет №5",
                        "Модуль №2, дискрет №6",
                        "Модуль №2, дискрет №7",
                        "Модуль №2, дискрет №8",
                        "Модуль №2, дискрет №9",
                        "Модуль №2, дискрет №10",
                        "Модуль №2, дискрет №11",
                        "Модуль №3, дискрет №1",
                        "Модуль №3, дискрет №2",
                        "Модуль №3, дискрет №3",
                        "Модуль №3, дискрет №4",
                        "Модуль №3, дискрет №5",
                        "Модуль №3, дискрет №6",
                        "Модуль №3, дискрет №7",
                        "Модуль №3, дискрет №8",
                        "Модуль №3, дискрет №9",
                        "Модуль №3, дискрет №10",
                        "Модуль №3, дискрет №11",
                        "Модуль №4, дискрет №1",
                        "Модуль №4, дискрет №2",
                        "Модуль №4, дискрет №3",
                        "Модуль №4, дискрет №4",
                        "Модуль №4, дискрет №5",
                        "Модуль №4, дискрет №6",
                        "Модуль №4, дискрет №7",
                        "Модуль №4, дискрет №8",
                        "Модуль №4, дискрет №9",
                        "Модуль №4, дискрет №10",
                        "Модуль №4, дискрет №11"
                    };
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Модуль №1, дискрет №1",
                        "Модуль №1, дискрет №2",
                        "Модуль №1, дискрет №3",
                        "Модуль №1, дискрет №4",
                        "Модуль №1, дискрет №5",
                        "Модуль №1, дискрет №6",
                        "Модуль №1, дискрет №7",
                        "Модуль №1, дискрет №8",
                        "Модуль №1, дискрет №9",
                        "Модуль №1, дискрет №10",
                        "Модуль №1, дискрет №11",
                        "Модуль №2, дискрет №1",
                        "Модуль №2, дискрет №2",
                        "Модуль №2, дискрет №3",
                        "Модуль №2, дискрет №4",
                        "Модуль №2, дискрет №5",
                        "Модуль №2, дискрет №6",
                        "Модуль №2, дискрет №7",
                        "Модуль №2, дискрет №8",
                        "Модуль №2, дискрет №9",
                        "Модуль №2, дискрет №10",
                        "Модуль №2, дискрет №11",
                        "Модуль №3, дискрет №1",
                        "Модуль №3, дискрет №2",
                        "Модуль №3, дискрет №3",
                        "Модуль №3, дискрет №4",
                        "Модуль №3, дискрет №5",
                        "Модуль №3, дискрет №6",
                        "Модуль №3, дискрет №7",
                        "Модуль №3, дискрет №8",
                        "Модуль №3, дискрет №9",
                        "Модуль №3, дискрет №10",
                        "Модуль №3, дискрет №11",
                        "Модуль №4, дискрет №1",
                        "Модуль №4, дискрет №2",
                        "Модуль №4, дискрет №3",
                        "Модуль №4, дискрет №4",
                        "Модуль №4, дискрет №5",
                        "Модуль №4, дискрет №6",
                        "Модуль №4, дискрет №7",
                        "Модуль №4, дискрет №8",
                        "Модуль №4, дискрет №9",
                        "Модуль №4, дискрет №10",
                        "Модуль №4, дискрет №11"
                    };
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICONGS_LIDA_4DISCRET)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Модуль №1, дискрет №1",
                        "Модуль №1, дискрет №2",
                        "Модуль №1, дискрет №3",
                        "Модуль №1, дискрет №4",
                        "Модуль №1, дискрет №5",
                        "Модуль №1, дискрет №6",
                        "Модуль №1, дискрет №7",
                        "Модуль №1, дискрет №8",
                        "Модуль №1, дискрет №9",
                        "Модуль №1, дискрет №10",
                        "Модуль №1, дискрет №11",
                        "Модуль №2, дискрет №1",
                        "Модуль №2, дискрет №2",
                        "Модуль №2, дискрет №3",
                        "Модуль №2, дискрет №4",
                        "Модуль №2, дискрет №5",
                        "Модуль №2, дискрет №6",
                        "Модуль №2, дискрет №7",
                        "Модуль №2, дискрет №8",
                        "Модуль №2, дискрет №9",
                        "Модуль №2, дискрет №10",
                        "Модуль №2, дискрет №11",
                        "Модуль №3, дискрет №1",
                        "Модуль №3, дискрет №2",
                        "Модуль №3, дискрет №3",
                        "Модуль №3, дискрет №4",
                        "Модуль №3, дискрет №5",
                        "Модуль №3, дискрет №6",
                        "Модуль №3, дискрет №7",
                        "Модуль №3, дискрет №8",
                        "Модуль №3, дискрет №9",
                        "Модуль №3, дискрет №10",
                        "Модуль №3, дискрет №11",
                        "Модуль №4, дискрет №1",
                        "Модуль №4, дискрет №2",
                        "Модуль №4, дискрет №3",
                        "Модуль №4, дискрет №4",
                        "Модуль №4, дискрет №5",
                        "Модуль №4, дискрет №6",
                        "Модуль №4, дискрет №7",
                        "Модуль №4, дискрет №8",
                        "Модуль №4, дискрет №9",
                        "Модуль №4, дискрет №10",
                        "Модуль №4, дискрет №11"
                    };
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICONGS_LIDA_2DISCRET)
                {
                    return new List<string>
                    {
                        "Нет",
                        "Модуль №1, дискрет №1",
                        "Модуль №1, дискрет №2",
                        "Модуль №1, дискрет №3",
                        "Модуль №1, дискрет №4",
                        "Модуль №1, дискрет №5",
                        "Модуль №1, дискрет №6",
                        "Модуль №1, дискрет №7",
                        "Модуль №1, дискрет №8",
                        "Модуль №1, дискрет №9",
                        "Модуль №1, дискрет №10",
                        "Модуль №1, дискрет №11",
                        "Модуль №2, дискрет №1",
                        "Модуль №2, дискрет №2",
                        "Модуль №2, дискрет №3",
                        "Модуль №2, дискрет №4",
                        "Модуль №2, дискрет №5",
                        "Модуль №2, дискрет №6",
                        "Модуль №2, дискрет №7",
                        "Модуль №2, дискрет №8",
                        "Модуль №2, дискрет №9",
                        "Модуль №2, дискрет №10",
                        "Модуль №2, дискрет №11"
                    };
                }
                else
                {
                    return new List<string>();
                };

            }
        }

    }
}
