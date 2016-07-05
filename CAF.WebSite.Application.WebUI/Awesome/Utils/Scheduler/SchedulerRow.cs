using System.Collections.Generic;

namespace AwesomeMvcDemo.Utils.Scheduler
{
    public class SchedulerRow
    {
        public string Time { get; set; }

        public string RowClass { get; set; }

        public string Date { get; set; }

        public IList<DayCell> Cells { get; set; }

        public bool AllDay { get; set; }

        public string Title { get; set; }

        public string Notes { get; set; }
    }
}