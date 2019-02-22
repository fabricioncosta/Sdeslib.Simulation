using System.Collections.Generic;
using System.Linq;

namespace Sdeslib.Simulation
{
    internal class EventList
    {
        //
        private readonly SortedList<Event, IEnumerator<Event>> sortedList = new SortedList<Event, IEnumerator<Event>>();

        public void Insert(IEnumerator<Event> enumerator)
        {
            if (enumerator.Current == null)
            {
                return;
            }

            sortedList.Add(enumerator.Current, enumerator);
        }

        public Event GetNext()
        {
            if (!sortedList.Any())
            {
                return null;
            }

            var first = sortedList.First().Value;
            var eve = sortedList.First().Key;
            sortedList.RemoveAt(0);

            if (first.MoveNext())
            {
                Insert(first);
            }
            
            return eve;
        }
        
    }
}