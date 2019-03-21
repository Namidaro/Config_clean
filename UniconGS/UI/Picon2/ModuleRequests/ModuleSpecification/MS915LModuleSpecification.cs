namespace UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification
{
    public class MS915LModuleSpecification : IModuleSpecification
    {
        public string DeviceName
        {
            get { return "Люксметр"; }
        }
        public byte ModuleType
        {
            get { return 0x0E; }
        }
        public byte Command
        {
            get
            {
                return 0x20;
            }
        }
        public ushort ModuleParameterAddress
        {
            get
            {
                return 0x0000;
            }
        }
        public ushort FirstModuleDatabaseAddress
        {
            get
            {
                return 0x001C;
            }
        }
        public byte ParameterCount
        {
            get
            {
                return 0x1E;
            }
        }
        public MS915LModuleSpecification()
        {

        }
    }
}
