using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace TestQuartz.Helper.Quartz
{
    public class QuartzStartup
    {
        //https://github.com/galyamichevp/pet/tree/42172c2c513f3a70366cb6ad7f7b17adcd38213d/netcore/exchanges/exchanges.server/Quartz
        
        private readonly IServiceProvider _serviceProvider;
        private  IScheduler _scheduler; // after Start, and until shutdown completes, references the scheduler object
        
        public QuartzStartup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        
        public async Task Start()
        {
            // other code is same
            _scheduler = await new StdSchedulerFactory().GetScheduler();
            _scheduler.JobFactory = new JobFactory(_serviceProvider);

            await _scheduler.Start();
            //var job = JobBuilder.Create<JobClass>().Build();
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<JobClass>()
                .WithIdentity("Job", "groupJob1")
                .Build();
            
            
            //var sampleTrigger = TriggerBuilder.Create().StartNow().WithCronSchedule("0 0/1 * * * ?").Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithCronSchedule("0/10 * * * * ?") // https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
//                .WithSimpleSchedule(x => x
//                    .WithIntervalInSeconds(1)
//                    .RepeatForever())
                .Build();
            
            
            await _scheduler.ScheduleJob(job, trigger);
        }
        
        public void Stop()
        {
            if (_scheduler == null)
            {
                return;
            }

            if (_scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
            {
                _scheduler = null;
            }
            else
            {
            }
        }
    }
}