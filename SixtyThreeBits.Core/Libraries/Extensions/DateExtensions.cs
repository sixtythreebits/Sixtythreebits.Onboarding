using System;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static partial class DateExtensions
    {
        #region Methods
        public static DateTime AddBusinessDays(this DateTime inputDate, int? days)
        {
            days = days ?? 0;

            if (days > 0)
            {
                while (days > 0)
                {
                    inputDate = inputDate.AddDays(1);
                    if (inputDate.DayOfWeek != DayOfWeek.Saturday && inputDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        --days;
                    }
                }
            }
            else if (days < 0)
            {
                while (days < 0)
                {
                    inputDate = inputDate.AddDays(-1);
                    if (inputDate.DayOfWeek != DayOfWeek.Saturday && inputDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        ++days;
                    }
                }
            }

            return inputDate;
        }
        public static DateTime? AddBusinessDays(this DateTime? inputDate, int? days)
        {
            return inputDate.HasValue ? inputDate.Value.AddBusinessDays(days) : null;
        }

        public static DateTime GetClosestWeekDayDate(this DateTime inputDate, int? dayOfWeek, DateTime? maxDate = null, DateTime? minDate = null)
        {
            var currentDayOfWeek = (int)inputDate.DayOfWeek;

            if (dayOfWeek == null || dayOfWeek < 0 || dayOfWeek > 7 || currentDayOfWeek == dayOfWeek)
            {
                return inputDate;
            }
            else
            {
                inputDate = inputDate.Date;
                if (dayOfWeek == 0)
                {
                    dayOfWeek = 7;
                }
                var daysToAdd = dayOfWeek.Value - currentDayOfWeek < 0 ? dayOfWeek.Value - currentDayOfWeek + 7 : dayOfWeek.Value - currentDayOfWeek;
                var daysToDeduct = currentDayOfWeek - dayOfWeek.Value < 0 ? currentDayOfWeek - dayOfWeek.Value + 7 : currentDayOfWeek - dayOfWeek.Value;

                var futureDate = inputDate.AddDays(daysToAdd);
                var pastDate = inputDate.AddDays(-daysToDeduct);

                if (daysToAdd <= daysToDeduct && (!maxDate.HasValue || (maxDate.HasValue && maxDate.Value >= futureDate)))
                {
                    return futureDate;
                }
                else if (!minDate.HasValue || (minDate.HasValue && pastDate >= minDate.Value))
                {
                    return pastDate;
                }
                else if (!maxDate.HasValue || (maxDate.HasValue && maxDate.Value >= futureDate))
                {
                    return futureDate;
                }

                return inputDate;
            }
        }
        public static DateTime? GetClosestWeekDayDate(this DateTime? inputDate, int? dayOfWeek, DateTime? maxDate = null, DateTime? minDate = null)
        {
            return inputDate.HasValue ? inputDate.Value.GetClosestWeekDayDate(dayOfWeek, maxDate, minDate) : null;
        }

        public static DateTime GetClosestNextBusinessDayOrSelf(this DateTime inputDate)
        {
            var result = inputDate;
            while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
            {
                result = result.AddDays(1);
            }
            return result;
        }
        public static DateTime? GetClosestNextBusinessDayOrSelf(this DateTime? inputDate)
        {
            return inputDate.HasValue ? inputDate.Value.GetClosestNextBusinessDayOrSelf() : null;            
        }

        public static DateTime GetClosestPrevBusinessDayOrSelf(this DateTime inputDate)
        {
            var result = inputDate;
            while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
            {
                result = result.AddDays(-1);
            }
            return result;
        }
        public static DateTime? GetClosestPrevBusinessDayOrSelf(this DateTime? inputDate)
        {
            return inputDate.HasValue ? inputDate.Value.GetClosestPrevBusinessDayOrSelf() : null;            
        }

        public static DateTime GetClosestPrevDayOfWeekOrSelf(this DateTime inputDate, DayOfWeek dayOfWeek)
        {
            var result = inputDate;
            while (result.DayOfWeek != dayOfWeek)
            {
                result = result.AddDays(-1);
            }
            return result;
        }
        public static DateTime? GetClosestPrevDayOfWeekOrSelf(this DateTime? inputDate, DayOfWeek dayOfWeek)
        {
            return inputDate.HasValue ? inputDate.Value.GetClosestPrevDayOfWeekOrSelf(dayOfWeek) : null;
        }

        public static DateTime GetClosestNextDayOfWeekOrSelf(this DateTime inputDate, DayOfWeek dayOfWeek)
        {
            var result = inputDate;
            while (result.DayOfWeek != dayOfWeek)
            {
                result = result.AddDays(1);
            }
            return result;
        }
        public static DateTime? GetClosestNextDayOfWeekOrSelf(this DateTime? inputDate, DayOfWeek dayOfWeek)
        {
            return inputDate.HasValue ? inputDate.Value.GetClosestNextDayOfWeekOrSelf(dayOfWeek) : null;
        }

        public static long ConvertToUnixTimeStamp(this DateTime inputDate)
        {
            var dateTimeOffset = new DateTimeOffset(inputDate.ToUniversalTime());
            var dateUnixTimeSeconds = dateTimeOffset.ToUnixTimeSeconds();
            return dateUnixTimeSeconds;
        }
        public static long? ConvertToUnixTimeStamp(this DateTime? inputDate)
        {
            return inputDate.HasValue ? inputDate.Value.ConvertToUnixTimeStamp() : null;
        }        
        #endregion Methods
    }
}