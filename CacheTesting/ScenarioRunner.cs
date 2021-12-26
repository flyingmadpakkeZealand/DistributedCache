using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheTesting
{
    public class ScenarioRunner
    {
        public void Run(TestScenarioBase testScenario, string testName)
        {
            Console.WriteLine($"Running {testName}(seed: {testScenario.Seed:N0})...");

            (int misses, int trueCacheMisses, int recurringRequestCount) = testScenario.RunTest();

            Console.WriteLine($"{testName} completed {testScenario.Iterations:N0} iterations with:");
            Console.WriteLine($"{misses:N0}/{testScenario.Iterations:N0} all cache misses ({(double) misses/testScenario.Iterations:P}).");
            Console.WriteLine($"{trueCacheMisses:N0}/{recurringRequestCount:N0} true cache misses ({(double) trueCacheMisses/recurringRequestCount:P}).\n");
        }
    }
}
