using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace TestQuartz.Helper.Quartz
{
    public class Quartz
    {

        private IScheduler _scheduler;
        

        public static IScheduler Scheduler { get { return Instance._scheduler; } }

        // Singleton
        private static Quartz _instance = null;
        
        /// <summary>
        /// Singleton
        /// </summary>
        public static Quartz Instance
        {
            get
            {
                if (_instance == null) {
                    _instance = new Quartz();
                }
                return _instance;
            }
        }

        private Quartz()
        {
            // Initialisieren
            Init();
        }

        private async void Init()
        {
            // Scheduler setzen mit standard Scheduler Factory
            _scheduler = await new StdSchedulerFactory().GetScheduler();
        }


        public IScheduler UseJobFactory(IJobFactory jobFactory)
        {
            Scheduler.JobFactory = jobFactory;
            return Scheduler;
        }


        public async void AddJob<T>(string name, string group, int interval)
            where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity( name, group )
                .Build();
            
            ITrigger jobTrigger = TriggerBuilder.Create()
                .WithIdentity( name + "Trigger", group )
                .StartNow() // Jetzt starten
                .WithSimpleSchedule( t => t.WithIntervalInSeconds( interval ).RepeatForever() ) 
                .Build();

           
            await Scheduler.ScheduleJob( job, jobTrigger );
        }

        
        public async void AddJob<T>(string name, string group, string cronSchedule)
            where T : IJob
        {
            // Job erstellen
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity( name, group )
                .Build();

            // Trigger erstellen
            ITrigger jobTrigger = TriggerBuilder.Create()
                .WithIdentity( name + "Trigger", group )
                .StartNow() // Jetzt starten
                .WithCronSchedule(cronSchedule) 
                .Build();

            // Job anfügen
            await Scheduler.ScheduleJob( job, jobTrigger );
        }

        public static async void Start()
        {
            await Scheduler.Start();
        }
    }
}