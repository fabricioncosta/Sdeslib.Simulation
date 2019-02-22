using System;
using System.Collections.Generic;

namespace Sdeslib.Simulation
{
    public class Event : IComparable<Event>
    {
        public Event(long timeStamp, Func<IEnumerable<Event>> doFunc)
        {
            this.TimeStamp = timeStamp;
            this.Do = doFunc;
        }

        private static int count = 0;
        public long Id { get; private set; } = count++;
        public long TimeStamp { get; private set; }

        public int CompareTo(Event other)
        {
            if (TimeStamp == other.TimeStamp)
            {
                return Id.CompareTo(other.Id);
            }

            return TimeStamp.CompareTo(other.TimeStamp);
        }

        public Func<IEnumerable<Event>> Do { get; private set; }
        public Event Parent { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(TimeStamp)}: {TimeStamp}, {nameof(Do)}: {Do}";
        }
    }
}