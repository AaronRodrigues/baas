using System;


namespace Energy.EHLCommsLibIntegrationTests.Services
{
    public static class SystemTime
    {
        private static Func<DateTime> _now = () => DateTime.Now;

        public static Func<DateTime> Set
        {
            set { _now = value; }
        }

        public static DateTime Now
        {
            get { return _now(); }
        }

        public static DateTime Today
        {
            get { return _now().Date; }
        }

        public static DateTime AddToSystemTime(string formattedTime)
        {
            var systemTime = Now;
            if (!string.IsNullOrWhiteSpace(formattedTime))
            {
                var cacheDurationItems = formattedTime.Split(':');
                if (cacheDurationItems.Length > 0)
                {
                    var days = GetDaysFromDuration(cacheDurationItems);
                    var hours = GetHoursFromDuration(cacheDurationItems);
                    var minutes = GetMinutesFromDuration(cacheDurationItems);

                    return systemTime.AddDays(days).AddHours(hours).AddMinutes(minutes);
                }
            }

            return systemTime;
        }

        #region Helper methods

        private static int GetDaysFromDuration(string[] cacheDurationItems)
        {
            if (cacheDurationItems.Length >= 1)
                return int.Parse(cacheDurationItems[0]);

            return 0;
        }

        private static int GetHoursFromDuration(string[] cacheDurationItems)
        {
            if (cacheDurationItems.Length >= 2)
                return int.Parse(cacheDurationItems[1]);

            return 0;
        }

        private static int GetMinutesFromDuration(string[] cacheDurationItems)
        {
            if (cacheDurationItems.Length >= 3)
                return int.Parse(cacheDurationItems[2]);

            return 0;
        }

        #endregion
    }
}
