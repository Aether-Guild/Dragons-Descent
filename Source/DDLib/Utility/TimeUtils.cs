using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD.Utility
{
    public class TimeUtils
    {
        public struct TimeTickKeep
        {
            public const int TicksToSeconds = GenTicks.TicksPerRealSecond;
            public const int SecondsToMinutes = 60;
            public const int MinutesToHours = 60;
            public const int HoursToDays = 24;
            public const int DaysToQuadrums = 15;
            public const int QuadrumsToYears = 4;

            public int ticks;
            public int seconds, minutes, hours, days, quadrums, years;

            public TimeTickKeep(TimeTickKeep keep) : this(keep.ticks, keep.seconds, keep.minutes, keep.hours, keep.days, keep.quadrums, keep.years) { }

            public TimeTickKeep(int seconds, int minutes, int hours) : this(0, seconds, minutes, hours, 0, 0, 0) {}

            public TimeTickKeep(int seconds, int minutes, int hours, int days, int quadrums, int years) : this(0, seconds, minutes, hours, days, quadrums, years) { }

            public TimeTickKeep(int ticks, int seconds, int minutes, int hours, int days, int quadrums, int years)
            {
                this.ticks = ticks;
                this.seconds = seconds;
                this.minutes = minutes;
                this.hours = hours;
                this.days = days;
                this.quadrums = quadrums;
                this.years = years;
            }

            public long ToTicks()
            {
                return CollectTicks(this);
            }

            public static TimeTickKeep FromTicks(long ticks)
            {
                TimeTickKeep keep = new TimeTickKeep();
                SliceTicks(ref keep, ticks);
                return keep;
            }

            public void Refresh()
            {
                SliceTicks(ref this, CollectTicks(this));
            }

            private static long CollectTicks(TimeTickKeep keep)
            {
                long time = keep.years * QuadrumsToYears; // 1 year is 4 quadrums
                time = (time + keep.quadrums) * DaysToQuadrums; //1 quadrum is 15 days
                time = (time + keep.days) * HoursToDays; //1 day is 24 hours
                time = (time + keep.hours) * MinutesToHours; //1 hour is 60 minutes
                time = (time + keep.minutes) * SecondsToMinutes; //1 minute is 60 seconds
                time = (time + keep.seconds) * TicksToSeconds; //This is the total number of seconds, next is to convert it to ticks.
                return time + keep.ticks;
            }

            private static void SliceTicks(ref TimeTickKeep keep, long ticks)
            {
                keep.years = 0;
                //ticks
                keep.ticks = (int)ticks % TicksToSeconds;
                ticks /= TicksToSeconds;
                //ticks->seconds
                keep.seconds = (int)ticks % SecondsToMinutes;
                ticks /= SecondsToMinutes;
                //seconds->minutes
                keep.minutes = (int)ticks % MinutesToHours;
                ticks /= MinutesToHours;
                //minutes->hours
                keep.hours = (int)ticks % HoursToDays;
                ticks /= HoursToDays;
                //hours->days
                keep.days = (int)ticks % DaysToQuadrums;
                ticks /= DaysToQuadrums;
                //days->quadrums
                keep.quadrums = (int)ticks % QuadrumsToYears;
                ticks /= QuadrumsToYears;
                //quadrums->years
                keep.years = (int)ticks; //Remainder is years.
            }
        }

        public class TimeTickSpan
        {
            private TimeTickKeep keep;

            public TimeTickSpan()
            {
                keep = new TimeTickKeep();
            }

            public TimeTickSpan(long ticks)
            {
                keep = TimeTickKeep.FromTicks(ticks);
            }

            public TimeTickSpan(TimeTickKeep span)
            {
                keep = new TimeTickKeep(span);
            }

            public int Ticks
            {
                get => keep.ticks;
                set
                {
                    keep.ticks = value;
                    keep.Refresh();
                }
            }

            public int Seconds
            {
                get => keep.seconds;
                set
                {
                    keep.seconds = value;
                    keep.Refresh();
                }
            }

            public int Minutes
            {
                get => keep.ticks;
                set
                {
                    keep.minutes = value;
                    keep.Refresh();
                }
            }

            public int Hours
            {
                get => keep.ticks;
                set
                {
                    keep.hours = value;
                    keep.Refresh();
                }
            }

            public int Days
            {
                get => keep.ticks;
                set
                {
                    keep.days = value;
                    keep.Refresh();
                }
            }

            public int Quadrums
            {
                get => keep.ticks;
                set
                {
                    keep.quadrums = value;
                    keep.Refresh();
                }
            }

            public int Years
            {
                get => keep.ticks;
                set
                {
                    keep.years = value;
                    keep.Refresh();
                }
            }
        }
    }
}
