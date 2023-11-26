
using System;

namespace PROGRAMMATION_SYST_ME.View
{
    class StatusView
    {
        public void JobStart(string jobName)
        {
            Console.WriteLine($"Job {jobName} start");
        }
        public void JobStop(string jobName, long elapsedTime)
        {
           Console.WriteLine($"Job {jobName} ended in {elapsedTime} milliseconds");
        }
        public void JobsComplete() 
        {
            Console.WriteLine("-> All save jobs are complete");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
