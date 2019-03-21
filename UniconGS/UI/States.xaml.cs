using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using UniconGS.Source;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for States.xaml
    /// </summary>
    public partial class States : UserControl, IQuery
    {
        private Slot _querer = null;
        private ushort[] _value = null;
        

        public States()
        {
            InitializeComponent();
        }

        #region Privates
        private void SetDefault()
        {
            this.uiStates.Children.OfType<Lightning>().ToList().ForEach((discret) => { discret.Value = null; });
            this.uiErrors.Children.OfType<Lightning>().ToList().ForEach((discret) => { discret.Value = null; });
        }

        private void SetDiscrets(List<bool> states, List<bool> errors)
        {
            var statesTmp = this.uiStates.Children.OfType<Lightning>().ToList();
            var discretsTmp = this.uiErrors.Children.OfType<Lightning>().ToList();
            for (int i = 0; i < statesTmp.Count; i++)
            {
                statesTmp[i].Value = states[i];
                discretsTmp[i].Value = errors[i];
            }
        }
        #endregion


        #region IQueryMember
        public Slot Querer
        {
            get
            {
                return this._querer;
            }
            set
            {
                this._querer = value;
            }
        }
        public ushort[] Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (value == null)
                {
                    this.SetDefault();
                }
                else
                {
                    this._value = value;
                    var bw1 = Converter.GetBitsFromWord(_value[0]).OfType<bool>().ToList().GetRange(0, 11);
                    var bw2 = Converter.GetBitsFromWord(_value[1]).OfType<bool>().ToList().GetRange(0, 11);
                    this.SetDiscrets(bw1, bw2);
                }            
            }
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public bool WriteContext()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
