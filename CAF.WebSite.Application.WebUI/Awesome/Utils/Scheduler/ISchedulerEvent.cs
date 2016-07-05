using System;

namespace AwesomeMvcDemo.Utils.Scheduler
{
    public class SchedulerEvent
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool AllDay { get; set; }

        public string Notes { get; set; }

        public string Color { get; set; }
    }
}