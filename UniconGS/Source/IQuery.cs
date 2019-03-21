namespace UniconGS.Source
{
    public interface IQuery
    {
        Slot Querer { get; set; }
        ushort[] Value { get; set; }
        void Update();
        bool WriteContext();
    }



}
