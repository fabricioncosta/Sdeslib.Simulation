using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sdeslib.Simulation;

namespace Sdeslib.Test
{
    [TestClass]
    public class SimulationTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            QueeueSimulation sim = new QueeueSimulation();
            var average = sim.Run();
            Assert.IsNotNull(average);
            Assert.AreEqual(0, average.Value, 2);
        }
    }
}