using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sdeslib.Simulation
{
    public class Simulation 
    {
        private readonly SortedSet<Event> pendingEvents;
        public long Now { get; private set; }        
        public Event NewEvent(long timeStemp, Func<IEnumerable<Event>> eventAction)
        {
            return new Event(timeStemp, eventAction) ;
        }
        public Simulation()
        {
            pendingEvents = new SortedSet<Event>();
        }
        private Event GetNextEvent()
        {
            if (!pendingEvents.Any())
            {
                return null;
            }
            
            var eve = pendingEvents.FirstOrDefault();
            pendingEvents.Remove(eve);
            return eve;
        }        
        public void InsertEvent(Event eve)
        {
            pendingEvents.Add(eve);            
        }
              
        public delegate void TickHandler(long now, Event lastEvent);

        public event TickHandler Tick;
        public long? MaxNumberOfIterations { get; set; } = null;
        public long? MaxSizeOfPendingEventList { get; set; } = null;
        public double? MaxTimeMin { get; set; } = null;

        private long iterations = 0;
        Stopwatch stopwatch = new Stopwatch();

        public Func<long, bool> UserStop { get; set; }

        public bool Stop()
        {
            if (MaxNumberOfIterations.HasValue && iterations > MaxNumberOfIterations)
            {
                return true;
            }

            if (MaxTimeMin.HasValue && stopwatch.ElapsedMilliseconds >= MaxTimeMin * 60 * 1000)
            {
                return true;
            }

            if (MaxSizeOfPendingEventList.HasValue && pendingEvents.Count > MaxSizeOfPendingEventList)
            {
                return true;
            }

            if (UserStop!=null && UserStop(Now))
            {
                return true;
            }

            return false;
        }
        
        private void RunI()
        {
            Now = 0;
            
            stopwatch.Start();
            
            do
            {
                Event currentEvent = GetNextEvent();
                Now = currentEvent.TimeStamp;

                if (currentEvent.Do != null)
                {
                    var newEvents = currentEvent.Do();

                    foreach (var newEvent in newEvents)
                    {
                        newEvent.Parent = currentEvent;
                        InsertEvent(newEvent);
                    }                    
                }

                Tick?.Invoke(Now, currentEvent);

                iterations++;
                
            } while (pendingEvents.Any() && !Stop());
        }
        
        EventList eventList2 = new EventList();

        public void Run()
        {
            Now = 0;

            stopwatch.Start();

            var eve = pendingEvents.GetEnumerator();

            if (eve.MoveNext())
            {
                eventList2.Insert(eve);
            }
            
            

            do
            {
                Event currentEvent = eventList2.GetNext();

                if (currentEvent == null)
                {
                    break;
                }

                Now = currentEvent.TimeStamp;

                if (currentEvent.Do != null)
                {
                    var newEvents = currentEvent.Do();
                    var enumerator = newEvents.GetEnumerator();

                    if (enumerator.MoveNext())
                    {
                        eventList2.Insert(enumerator);
                    }
                    
                }

                Tick?.Invoke(Now, currentEvent);

                iterations++;

            } while (pendingEvents.Any() && !Stop());
        }

        public void Reset()
        {
            this.Now = 0;
            this.pendingEvents.Clear();
        }

    }
}