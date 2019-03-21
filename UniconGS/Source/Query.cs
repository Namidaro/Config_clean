using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniconGS.UI;

namespace UniconGS.Source
{
    /// <summary>
    /// Класс, который содержит управляет запросами
    /// </summary>
    public class Query
    {
        public bool IsCycle { get; set; }
        public Accsess Operation { get; set; }
        private IQuery _control;
        private bool v;
        private Accsess read;
        //private Runo3Diagnostics uiRuno3Diagnostics;

        public IQuery Control
        {
            get { return _control; }
        }

        public Query(IQuery control, bool cycle, Accsess op)
        {
            _control = control;
            IsCycle = cycle;
            Operation = op;
        }

        //public Query(Runo3Diagnostics uiRuno3Diagnostics, bool v, Accsess read)
        //{
        //    this.uiRuno3Diagnostics = uiRuno3Diagnostics;
        //    this.v = v;
        //    this.read = read;
        //}

        public void Update()
        {
            _control?.Update();
        }

        public void WriteContext()
        {
            _control?.WriteContext();
        }

       
    }
    /// <summary>
    /// Определяет, какая операция будет производиться
    /// </summary>
    public enum Accsess
    {
        Read,
        Write
    }
}
