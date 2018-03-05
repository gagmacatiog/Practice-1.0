
namespace ServicingTerminalApplication
{
    public class _Main_Queue
    {
        public string Key { get; set; }
        public int Queue_Number { get; set; }
        public string Full_Name { get; set; }
        public int Servicing_Office { get; set; }
        public int Mode { get; internal set; }
        public int Transaction_Type { get; set; }
        // Student_No is stored as string, because of dashes and possible value 'Guest'
        public string Student_No { get; set; }
        public int Pattern_Current { get; set; }
        public int ID { get; set; }

    }
}