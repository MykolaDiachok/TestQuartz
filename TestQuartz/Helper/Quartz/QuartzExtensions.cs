using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace TestQuartz.Helper.Quartz
{
    public static class QuartzExtensions
    {
        public static void UseQuartz(this IApplicationBuilder app, Action<Quartz> configuration)
        {
            var jobFactory = (IJobFactory)app.ApplicationServices.GetService( typeof( IJobFactory ) );
            // Set job factory
            Quartz.Instance.UseJobFactory( jobFactory );

            // Run configuration
            configuration.Invoke( Quartz.Instance );
            // Run Quartz
            Quartz.Start();
        }

        public static async void AddQuartz(this IServiceCollection services)
        {
            var props = new NameValueCollection
            {
                {"quartz.serializer.type", "json"}
            };
            var factory = new StdSchedulerFactory(props);
            var scheduler = await factory.GetScheduler();

            var jobFactory = new JobFactory(services.BuildServiceProvider());
            scheduler.JobFactory = jobFactory;
            await scheduler.Start();
            services.AddSingleton(scheduler);
        }
    }
}