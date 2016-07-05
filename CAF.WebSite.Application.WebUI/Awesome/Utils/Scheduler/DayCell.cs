namespace AwesomeMvcDemo.Utils.Scheduler
{
    public class DayCell
    {
        public long Ticks { get; set; }

        public EventDisplay[] Events { get; set; }

        public string Day { get; set; }

        public string Date { get; set; }
    }
}