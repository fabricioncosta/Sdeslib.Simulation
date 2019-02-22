using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sdeslib.Simulation
{
    public class QueeueSimulation
    {
        Simulation sim = new Simulation();
        private double averageCustomerArrivalTime = 10;
        private double averageCustomerServiceTime = 10;
        private Queue<string> clientQueue { get; set; } = new Queue<string>();
        public bool InAttendance { get; set; }

        public class Item
        {
            public string Client { get; set; }
            public long? Arrival { get; set; }
            public long? Start { get; set; }
            public long? Finish { get; set; }
            public long? WaitTime => Start - Arrival;
        }

        public Dictionary<string, Item> Attendance = new Dictionary<string, Item>();

        public IEnumerable<Event> AddAttendanceQueue(string client)
        {
            Console.WriteLine($"{sim.Now,-10} AddAttendanceQueue {client}");
            var now = sim.Now;
            clientQueue.Enqueue(client);
            yield break;
        }

        public IEnumerable<Event> StartService(string client)
        {
            Attendance[client].Start = sim.Now;
            Console.WriteLine($"{sim.Now,-10} StartService {client}");
            var now = sim.Now;

            yield return sim.NewEvent(now + FactoryRandom.GetPoisson(averageCustomerServiceTime), () => EndService(client));
        }

        public IEnumerable<Event> EndService(string client)
        {
            Attendance[client].Finish = sim.Now;
            Console.WriteLine($"{sim.Now,-10} EndService {client}");
            InAttendance = false;

            if (clientQueue.Any())
            {
                var c = clientQueue.Dequeue();

                yield return sim.NewEvent(sim.Now, () => StartService(c));
            }
        }

        public IEnumerable<Event> ArriveClient(string client)
        {
            Attendance.Add(client, new Item {Arrival = sim.Now});
            Console.WriteLine($"{sim.Now,-10} ArriveClient {client} ");
            var now = sim.Now;

            if (!InAttendance && !clientQueue.Any())
            {
                InAttendance = true;

                yield return sim.NewEvent(sim.Now, () => StartService(client));
            }
            else
            {
                yield return sim.NewEvent(sim.Now, () => AddAttendanceQueue(client));
            }
        }

        private int count = 0;

        public IEnumerable<Event> GenerateCustomers()
        {
            var now = sim.Now;

            while (true)
            {
                now += FactoryRandom.GetPoisson(averageCustomerArrivalTime);

                yield return sim.NewEvent(now, () => ArriveClient($"client {count++}"));
            }
        }

        public double? Run()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sim.InsertEvent(sim.NewEvent(FactoryRandom.GetPoisson(averageCustomerArrivalTime), () => GenerateCustomers()));
            sim.UserStop += x => x >= 30000;
            sim.Run();
            var averateWaitingTime = Attendance.Where(x => x.Value.Start.HasValue).Average(x => x.Value.WaitTime);
            Console.WriteLine($"Average waiting time: {averateWaitingTime}");
            Console.WriteLine(sw.Elapsed);
            return averateWaitingTime.Value;
        }
    }



}