using System;
using System.ComponentModel;
using PirateKing.Constants;

namespace PirateKing.Core
{
    [TypeConverter(typeof(DateTypeConverter))]
    public sealed class Date : IEquatable<Date>, IComparable<Date>
    {
        private readonly DateTime dateTime;

        public Date(DateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public Date(string dateTime)
        {
            if (DateTime.TryParse(dateTime, out DateTime date) == false)
            {
                throw new ArgumentException($"Invalid {nameof(dateTime)} {dateTime}");
            }

            this.dateTime = date;
        }

        public Date AddDays(int days)
        {
            DateTime dateTime = this.dateTime.AddDays(days);
            return new Date(dateTime);
        }

        public DateTime ToDateTime() => this.dateTime;

        public override string ToString() => dateTime.ToString(DateFormats.DateYmd);

        public bool Equals(Date other) 
            => other != null && this.ToString() == other.ToString();

        public int CompareTo(Date other)
        {
            return this.dateTime.CompareTo(other.dateTime);
        }
    }
}
