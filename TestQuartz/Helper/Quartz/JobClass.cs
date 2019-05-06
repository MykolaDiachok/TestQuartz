using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace TestQuartz.Helper.Quartz
{
    //https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/more-about-jobs.html
    //https://www.youtube.com/watch?v=_mdZyH41ULs
    
    [DisallowConcurrentExecution]
    public class JobClass : IJob
    {
        
        private readonly ILogger<JobClass> _logger;

        public JobClass(ILogger<JobClass> logger)
        {
            _logger = logger;
        }
        
        
        public Task Execute(IJobExecutionContext context)
        {
            using (_logger.BeginScope($"Job scope {Guid.NewGuid()}"))
            {
                _logger.LogDebug("Start execute called");
                Thread.Sleep(3000);
                _logger.LogDebug("End execute called");
            }
            
            return Task.CompletedTask;
        }
    }
}