using CAF.Infrastructure.MvcHtml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;


namespace AwesomeMvcDemo.Utils.Scheduler
{
    public class SchedulerModelBuilder
    {
        public SchedulerModelBuilder(GridParams gridParams)
        {
            this.gridParams = gridParams;
            Culture = Thread.CurrentThread.CurrentCulture;
            viewType = SchedulerView.Week;
        }

        private readonly GridParams gridParams;

        private SchedulerView viewType;

        private DateTimeFormatInfo Dformat { get { return Culture.DateTimeFormat; } }

        public CultureInfo Culture { get; set; }

        public Func<DateTime, DateTime, IEnumerable<SchedulerEvent>> GetEvents { get; set; }

        public SchedulerView? ViewType
        {
            get
            {
                return viewType;
            }
            set
            {
                viewType = value ?? viewType;
            }
        }

        public SchedulerHour? HoursType { get; set; }

        public int? HourStep { get; set; }

        public string Cmd { get; set; }

        public int MinutesOffset { get; set; }

        public DateTime? Date { get; set; }

        public static IEnumerable<KeyContent> GetViewTypes()
        {
            var viewTypes = new Dictionary<SchedulerView, string>
                {
                    {SchedulerView.Day, "Day"}, 
                    {SchedulerView.Week, "Week"}, 
                    {SchedulerView.WorkWeek, "Work Week"}, 
                    {SchedulerView.Month, "Month"}, 
                    {SchedulerView.Agenda, "Agenda"}
                };

            return viewTypes.Select(o => new KeyContent(o.Key.ToString(), o.Value));
        }

        public static IEnumerable<KeyContent> GetHoursTypes()
        {
            var hoursTypes = new Dictionary<SchedulerHour, string>
                {
                    { SchedulerHour.FullDay, "full day" },
                    { SchedulerHour.WorkingHours, "work hours" },
                };

            return hoursTypes.Select(o => new KeyContent(o.Key, o.Value));
        }

        public static IEnumerable<KeyContent> GetHourSteps()
        {
            var hoursTypes = new Dictionary<int, string>
                {
                    { 30, "30 min" },
                    { 60, "1 hour" },
                    { 120, "2 hours" },
                    { 180, "3 hours" },
                    { 300, "5 hours" },
                    { 720, "12 hours" },
                };

            return hoursTypes.Select(o => new KeyContent(o.Key, o.Value));
        }

        public GridModel<SchedulerRow> Build()
        {
            var clientDate = HandleCommand();

            DateTime periodStartClient, periodEndClient;

            GetPeriodForViewType(clientDate, out periodStartClient, out periodEndClient);

            var periodStartUtc = periodStartClient.AddMinutes(MinutesOffset);
            var periodEndUtc = periodEndClient.AddMinutes(MinutesOffset);

            var columns = GenerateColumnsForViewType(periodStartClient, periodEndClient);

            gridParams.Columns = columns.ToArray();
            gridParams.Paging = false;

            var events = GetEvents(periodStartUtc, periodEndUtc).ToArray();

            var rows = new List<SchedulerRow>();

            if (IsDayLikeView())
            {
                var allDayRow = new SchedulerRow { Time = "all day", AllDay = true };

                var allDayCells = new List<DayCell>();
                var daysCount = GetDaysCount();

                for (var i = 0; i < daysCount; i++)
                {
                    var dayStartUtc = periodStartUtc.AddDays(i);
                    var allDayEvents = GetAllDayEvents(events, dayStartUtc);
                    var cell = BuildCell(allDayEvents, dayStartUtc, ViewType.Value);
                    allDayCells.Add(cell);
                }

                allDayRow.Cells = allDayCells;

                rows.Add(allDayRow);
            }

            if (ViewType == SchedulerView.Month)
            {
                var weeksInPeriod = Math.Ceiling((periodEndUtc - periodStartUtc).TotalDays / 7);
                for (var week = 0; week < weeksInPeriod; week++)
                {
                    var row = new SchedulerRow();
                    var cells = new List<DayCell>();

                    for (var day = 0; day < 7; day++)
                    {
                        var dayEvents = GetDayEvents(events, periodStartUtc.AddDays(day + week * 7));
                        var cell = BuildCell(dayEvents, periodStartUtc.AddDays(day + week * 7), ViewType.Value);
                        cells.Add(cell);
                    }

                    row.Cells = cells;
                    rows.Add(row);
                }
            }
            else if (ViewType == SchedulerView.Agenda)
            {
                for (var day = periodStartUtc; day < periodEndUtc; day = day.AddDays(1))
                {
                    var clientDay = day.AddMinutes(-MinutesOffset);

                    var dayEvents = GetDayEvents(events, day);
                    rows.AddRange(dayEvents.Select(o => new SchedulerRow
                    {
                        Date = clientDay.ToString(Dformat.LongDatePattern),
                        Time = AgendaTime(o, clientDay, MinutesOffset, Dformat),
                        Title = o.Title,
                        Notes = o.Notes,
                        RowClass = "agendaRow"
                    }));
                }
            }
            else if (IsDayLikeView())
            {
                var minutes = 0;
                var minutesUpperBound = 24 * 60;
                var stepMinutes = HourStep ?? 30;

                if (HoursType == SchedulerHour.WorkingHours)
                {
                    minutes = 8 * 60;
                    minutesUpperBound = 20 * 60;
                }

                while (minutes < minutesUpperBound)
                {
                    var row = new SchedulerRow();
                    var partHour = minutes / 60;
                    var partMinute = minutes % 60;

                    var timePartStartUtc =
                        new DateTime(
                            periodStartClient.Year,
                            periodStartClient.Month,
                            periodStartClient.Day,
                            partHour,
                            partMinute,
                            0).AddMinutes(MinutesOffset);

                    // any date will do, only time is shown
                    row.Time = (DateTime.UtcNow.Date + new TimeSpan(partHour, partMinute, 0)).ToString(Dformat.ShortTimePattern);

                    var days = GetDaysCount();
                    var cells = new List<DayCell>();

                    for (var day = 0; day < days; day++)
                    {
                        var startUtc = timePartStartUtc.AddDays(day);
                        
                        var allDayIds = rows[0].Cells[day].Events.Select(o => o.Id);
                        var timePartEvents = GetTimePartEvents(
                            events, startUtc, startUtc.AddMinutes(stepMinutes), allDayIds);

                        var cell = BuildCell(timePartEvents, startUtc, ViewType.Value);

                        cells.Add(cell);
                    }

                    minutes += stepMinutes;
                    row.Cells = cells;
                    rows.Add(row);
                }
            }

            var model = new GridModelBuilder<SchedulerRow>(rows.AsQueryable(), gridParams)
            {
                FrozenRows = IsDayLikeView() ? 1 : 0
            }.Build();

            model.Tag = new
            {
                View = ViewType.ToString(),
                Date = clientDate.ToString(Dformat.ShortDatePattern),
                DateLabel = GetDateStr(clientDate, periodStartClient, periodEndClient)
            };

            return model;
        }

        private bool IsDayLikeView()
        {
            return ViewType == SchedulerView.Day || ViewType == SchedulerView.Week || ViewType == SchedulerView.WorkWeek;
        }

        private string GetDateStr(DateTime clientDate, DateTime periodStartClient, DateTime periodEndClient)
        {
            var datestr = clientDate.ToString(Dformat.LongDatePattern);

            if (ViewType == SchedulerView.Week || ViewType == SchedulerView.WorkWeek)
            {
                datestr = periodStartClient.ToString(Dformat.MonthDayPattern) + " - " + periodEndClient.Day + ", " + periodEndClient.Year;
            }
            else if (ViewType == SchedulerView.Month)
            {
                datestr = clientDate.ToString(Dformat.YearMonthPattern);
            }

            return datestr;
        }

        private List<Column> GenerateColumnsForViewType(DateTime periodStartClient, DateTime periodEndClient)
        {
            var columns = new List<Column>();

            if (ViewType == SchedulerView.Day || ViewType == SchedulerView.Week || ViewType == SchedulerView.WorkWeek)
            {
                columns.Add(new Column { ClientFormat = "<div class='timeLabel'>.Time</div>", Width = 90 });
            }

            if (ViewType == SchedulerView.Agenda)
            {
                columns.Add(new Column { Name = "Date", Group = true });
                columns.Add(new Column { Name = "Time", Width = 200 });
                columns.Add(new Column { Name = "Title", Header = "Event" });
                columns.Add(new Column { Name = "Notes" });
            }
            else
            {
                var index = 0;
                for (var day = periodStartClient; day < periodEndClient && index < 7; day = day.AddDays(1))
                {
                    var header = ViewType == SchedulerView.Month
                                     ? Dformat.GetDayName(day.DayOfWeek)
                                     : Dformat.GetAbbreviatedDayName(day.DayOfWeek) + " "
                                       + Dformat.GetAbbreviatedMonthName(day.Date.Month) + " " + day.Day;

                    // week headers are clickable
                    if (ViewType == SchedulerView.Week || ViewType == SchedulerView.WorkWeek)
                    {
                        header = string.Format("<span class='day' data-date='{1}'>{0}</a>", header, day.ToShortDateString());
                    }

                    var column = new Column { ClientFormatFunc = "buildCell(" + index + ")", Header = header };
                    columns.Add(column);
                    index++;
                }
            }

            return columns;
        }

        private void GetPeriodForViewType(DateTime clientDate, out DateTime periodStartClient, out DateTime periodEndClient)
        {
            if (ViewType == SchedulerView.Day)
            {
                periodStartClient = clientDate.Date;
                periodEndClient = periodStartClient.AddDays(1);
            }
            else if (ViewType == SchedulerView.WorkWeek)
            {
                periodStartClient = StartOfWeek(clientDate, DayOfWeek.Monday).Date;
                periodEndClient = periodStartClient.AddDays(5);
            }
            else if (ViewType == SchedulerView.Week)
            {
                periodStartClient = StartOfWeek(clientDate, Dformat.FirstDayOfWeek).Date;
                periodEndClient = periodStartClient.AddDays(7);
            }
            else if (ViewType == SchedulerView.Agenda)
            {
                periodStartClient = clientDate.Date;
                periodEndClient = periodStartClient.AddDays(7);
            }
            else // MonthView
            {
                periodStartClient = StartOfWeek(new DateTime(clientDate.Year, clientDate.Month, 1), Dformat.FirstDayOfWeek).Date;
                periodEndClient = StartOfWeek(new DateTime(clientDate.Year, clientDate.Month, DateTime.DaysInMonth(clientDate.Year, clientDate.Month)), Dformat.FirstDayOfWeek).AddDays(7);
            }
        }

        private DateTime HandleCommand()
        {
            var clientToday = DateTime.UtcNow.AddMinutes(-MinutesOffset);
            var clientDate = Date ?? clientToday;

            if (Cmd != null)
            {
                if (Cmd == "today")
                {
                    clientDate = clientToday;
                }
                else
                {
                    var delta = Cmd == "next" ? 1 : -1;

                    if (ViewType == SchedulerView.Day || ViewType == SchedulerView.Agenda)
                    {
                        clientDate = clientDate.AddDays(delta);
                    }
                    else if (ViewType == SchedulerView.Month)
                    {
                        clientDate = clientDate.AddMonths(delta);
                    }
                    else if (ViewType == SchedulerView.WorkWeek)
                    {
                        clientDate = clientDate.AddDays(delta * 5);
                    }
                    else if (ViewType == SchedulerView.Week)
                    {
                        clientDate = clientDate.AddDays(delta * 7);
                    }
                }
            }
            return clientDate;
        }

        private static IEnumerable<SchedulerEvent> GetDayEvents(IEnumerable<SchedulerEvent> events, DateTime dayStartUtc)
        {
            var dayEndUtc = dayStartUtc.AddDays(1);

            var dayEvents = events.Where(o => o.Start < dayEndUtc && o.End > dayStartUtc);

            return dayEvents;
        }

        private IEnumerable<SchedulerEvent> GetAllDayEvents(SchedulerEvent[] events, DateTime dayStartUtc)
        {
            var dayEndUtc = dayStartUtc.AddDays(1);

            var result = new List<SchedulerEvent>();
            result.AddRange(events.Where(o => o.AllDay
                && o.Start < dayEndUtc
                && o.End >= dayStartUtc));

            // events which duration during the day is longer than 7 hours
            var nonMarked = events.Where(o => !o.AllDay && ((dayEndUtc < o.End ? dayEndUtc : o.End) - (dayStartUtc > o.Start ? dayStartUtc : o.Start)).TotalHours >= 7);
            
            result.AddRange(nonMarked);

            return result;
        }

        private static IEnumerable<SchedulerEvent> GetTimePartEvents(IEnumerable<SchedulerEvent> events, DateTime startUtc, DateTime endUtc, IEnumerable<int> allDayIds)
        {
            var timePartEvents = events.Where(o => !o.AllDay && o.Start < endUtc && o.End > startUtc && !allDayIds.Contains(o.Id));

            return timePartEvents;
        }

        private static string AgendaTime(SchedulerEvent events, DateTime clientDay, int minutesOffset, DateTimeFormatInfo cformat)
        {
            if (events.AllDay) return "all day";

            var startDateClient = events.Start.AddMinutes(-minutesOffset);
            var endDateClient = events.End.AddMinutes(-minutesOffset);

            if (startDateClient.Date == endDateClient.Date)
            {
                return string.Format("{0} - {1}", startDateClient.ToString(cformat.ShortTimePattern), endDateClient.ToString(cformat.ShortTimePattern));
            }

            if (startDateClient.Date == clientDay.Date)
            {
                return startDateClient.ToString(cformat.ShortTimePattern) + " - ...";
            }

            return "... - " + startDateClient.ToString(cformat.ShortTimePattern);
        }

        private DateTime UtcToClient(DateTime utc)
        {
            return utc.AddMinutes(-MinutesOffset);
        }

        private DayCell BuildCell(IEnumerable<SchedulerEvent> events, DateTime startUtc, SchedulerView view)
        {
            var cell = new DayCell { Ticks = startUtc.Ticks };
            var startClient = UtcToClient(startUtc);

            if (view == SchedulerView.Month)
            {
                cell.Day = startClient.Day == 1 ? startClient.ToString("d MMM") : startClient.Day.ToString(CultureInfo.InvariantCulture);
                cell.Date = startClient.Date.ToString(Dformat.ShortDatePattern);
            }

            var values = new List<EventDisplay>();

            foreach (var ev in events)
            {
                var ed = new EventDisplay { Id = ev.Id, Title = ev.Title, Color = ev.Color };

                var clientStart = UtcToClient(ev.Start);
                var clientEnd = UtcToClient(ev.End);

                if (ev.AllDay)
                {
                    ed.Time = "";
                }
                else if (clientStart.Date == clientEnd.Date || clientEnd.Date == clientStart.Date.AddDays(1))
                {
                    ed.Time = clientStart.ToString(Dformat.ShortTimePattern) + "-"
                              + clientEnd.ToString(Dformat.ShortTimePattern);
                }
                else
                {
                    if(startClient.Date == ev.Start.Date)
                        ed.Time = clientStart.ToString(Dformat.ShortTimePattern) + " - ...";
                    else
                        ed.Time = "... - " + clientStart.ToString(Dformat.ShortTimePattern);
                }

                values.Add(ed);
            }

            cell.Events = values.ToArray();

            return cell;
        }

        private static DateTime StartOfWeek(DateTime dt, DayOfWeek fdow)
        {
            return dt.AddDays(-(dt.DayOfWeek - fdow));
        }

        private int GetDaysCount()
        {
            return ViewType == SchedulerView.Day ? 1 : ViewType == SchedulerView.WorkWeek ? 5 : 7;
        }
    }
}