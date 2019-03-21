using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace UniconGS.UI.Picon2.ViewModel
{
    public class DateEnterFieldViewModel : BindableBase
    {
        private const string EMPTY_IP = "...";
        private string _datePart1;


        public string DatePart1
        {
            get { return _datePart1; }
            set
            {
                int intVal = 0;
                if (int.TryParse(value, out intVal))
                {
                    if ((intVal <= 31) && (intVal >= 0)) _datePart1 = value;
                    else if (intVal > 31) _datePart1 = "31";
                    else if (intVal < 0) _datePart1 = "0";
                }
                else if (String.IsNullOrEmpty(value))
                {
                    _datePart1 = string.Empty;
                }
                OnPropertyChanged(nameof(DatePart1));
            }
        }

        private string _datePart2;

        public string DatePart2
        {
            get { return _datePart2; }
            set
            {
                int intVal = 0;
                if (int.TryParse(value, out intVal))
                {
                    if ((intVal <= 12) && (intVal >= 0)) _datePart2 = value;
                    else if (intVal > 12) _datePart2 = "12";
                    else if (intVal < 0) _datePart2 = "0";
                }
                else if (String.IsNullOrEmpty(value))
                {
                    _datePart2 = string.Empty;
                }
                OnPropertyChanged(nameof(DatePart2));
            }
        }




        public string FullDate
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                _datePart1 = _datePart1.Replace(" ", "");
                sb.Append(_datePart1);
                sb.Append(".");
                _datePart2 = _datePart2.Replace(" ", "");
                sb.Append(_datePart2);
                sb.Append(".");
                return sb.ToString();
            }
        }



        public DateEnterFieldViewModel()
        {


        }


    }
}
