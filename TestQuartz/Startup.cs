using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.Quartz;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz.Spi;
using TestQuartz.Helper.Quartz;

namespace TestQuartz
{
    public class Startup
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.2
        private readonly ILogger _logger;
        
        private IServiceCollection _services;
        
        
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddTransient<JobClass>();
            services.AddTransient<IJobFactory, JobFactory>(
                (provider) => new JobFactory( provider ));
            //services.AddTransient<JobClass>(); 
        }

        public IContainer ApplicationContainer { get; private set; }
        
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
//            var quartz = new QuartzStartup(_services.BuildServiceProvider());
//            applicationLifetime.ApplicationStarted.Register(() => quartz.Start().Wait());
//            applicationLifetime.ApplicationStopped.Register(quartz.Stop);
            
            //https://stackoverflow.com/questions/28258227/how-to-set-environment-name-ihostingenvironment-environmentname
            _logger.LogInformation($"In {env.EnvironmentName} environment");
            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseQuartz((quartz) => {quartz.AddJob<JobClass>("job","group1","0/10 * * * * ?");});
            
            
        }
        
        

        
    }
}