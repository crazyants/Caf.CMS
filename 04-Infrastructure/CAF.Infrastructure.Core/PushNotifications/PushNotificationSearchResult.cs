﻿using System.Collections.Generic;

namespace CAF.Infrastructure.Core.PushNotifications
{
    public class PushNotificationSearchResult
    {
        public PushNotificationSearchResult()
        {
            NotifyEvents = new List<PushNotification>();
        }
        public int TotalCount { get; set; }
        public int NewCount { get; set; }
        public List<PushNotification> NotifyEvents { get; set; }
    }
}
