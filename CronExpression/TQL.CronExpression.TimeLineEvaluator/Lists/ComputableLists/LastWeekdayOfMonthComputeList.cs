﻿using System;
using TQL.CronExpression.Parser.Utils;

namespace TQL.CronExpression.TimelineEvaluator.Lists.ComputableLists
{
    public class LastWeekdayOfMonthComputeList : DateTimeBasedComputeList
    {
        public LastWeekdayOfMonthComputeList(Ref<DateTimeOffset> referenceTime)
            : base(referenceTime, null)
        {
        }

        public override int Count => 1;

        public override int Element(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            var refTime = ReferenceTime.Value;
            var daysInMonth = DateTime.DaysInMonth(refTime.Year, refTime.Month);
            var day = new DateTime(refTime.Year, refTime.Month, daysInMonth);
            if (day.DayOfWeek == DayOfWeek.Saturday)
                return daysInMonth - 1;
            if (day.DayOfWeek == DayOfWeek.Sunday)
                return daysInMonth - 2;
            return daysInMonth;
        }
    }
}