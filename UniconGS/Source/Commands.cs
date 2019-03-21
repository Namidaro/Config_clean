using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniconGS.Source
{
    public class Commands
    {
        public class StartUpdateEventHandler
        {
            private object startUpdate;

            public StartUpdateEventHandler(object startUpdate)
            {
                this.startUpdate = startUpdate;
            }
        }

        public class StopUpdateEventHandler
        {
            private object stopUpdate;

            public StopUpdateEventHandler(object stopUpdate)
            {
                this.stopUpdate = stopUpdate;
            }
        }

        public delegate void ReportStatusEventHandler(string message);
    }
}
