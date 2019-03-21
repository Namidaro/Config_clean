using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace UniconGS.ViewModel
{
   public class DiagnosticTabViewModel:BindableBase
    {
        private int _gsmSignalLevel;

        public int GsmSignalLevel
        {
            get { return _gsmSignalLevel; }
            set
            {
                _gsmSignalLevel = value;
                RaisePropertyChanged();
            }
        }

        private void UpdateGsmLevel()
        {
            
        }


    }
}
