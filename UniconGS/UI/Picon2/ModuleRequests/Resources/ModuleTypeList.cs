using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniconGS.Enums;

namespace UniconGS.UI.Picon2.ModuleRequests.Resources
{
    public class ModuleTypeList
    {
        public SortedList<string, byte> ModuleList = new SortedList<string, byte>();

        public ModuleTypeList()
        {
            InitializeModuleList();
        }

        private void InitializeModuleList()
        {
            //ВНИМАНИЕ! буква 'М' в стрингах записана латинницей, чтобы была нормальная сортировка. Просто мне слишком лень делать велосипеды
            ModuleList.Add("#Заглушка", (byte)(ModuleSelectionEnum.MODULE_EMPTY));
            ModuleList.Add("MРВ960", (byte)(ModuleSelectionEnum.MODULE_MRV960));
            ModuleList.Add("MРВ980", (byte)(ModuleSelectionEnum.MODULE_MRV980));
            ModuleList.Add("MСД980", (byte)(ModuleSelectionEnum.MODULE_MSD980));
            ModuleList.Add("MСА961", (byte)(ModuleSelectionEnum.MODULE_MSA961));
            ModuleList.Add("MСА962", (byte)(ModuleSelectionEnum.MODULE_MSA962));
            ModuleList.Add("MИИ901", (byte)(ModuleSelectionEnum.MODULE_MII901));
            ModuleList.Add("MС911", (byte)(ModuleSelectionEnum.MODULE_MS911));
            ModuleList.Add("MС911Р", (byte)(ModuleSelectionEnum.MODULE_MS911R));
            ModuleList.Add("MС910Р", (byte)(ModuleSelectionEnum.MODULE_MS910R));
            ModuleList.Add("MС915", (byte)(ModuleSelectionEnum.MODULE_MS915));
            ModuleList.Add("MС917", (byte)(ModuleSelectionEnum.MODULE_MS917));
            ModuleList.Add("Счетчик", (byte)(ModuleSelectionEnum.MODULE_MS915C));
            ModuleList.Add("Люксметр", (byte)(ModuleSelectionEnum.MODULE_MS915L));
        }
    }
}
