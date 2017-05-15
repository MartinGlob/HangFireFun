using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Topshelf;
using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.Mongo;
using Topshelf.Options;
using Microsoft.Owin.Hosting;

[assembly: OwinStartup(typeof(app.Startup))]

namespace app
{
    class Program
    {
        //[assembly: OwinStartup(typeof(WindowsService.Startup))]
        static void Main(string[] args)
        {

           
            HostFactory.Run(x =>                                 //1
            {
                x.Service<TownCrier>(s =>                        //2
                {
                    s.ConstructUsing(name => new TownCrier());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6

                x.SetDescription("Sample Topshelf Host");        //7
                x.SetDisplayName("Stuff");                       //8
                x.SetServiceName("Stuff");                       //9
            });
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireDashboard();
        }
    }

    public class TownCrier
    {
        readonly Timer _timer;
        public TownCrier()
        {

            GlobalConfiguration.Configuration.UseMongoStorage("mongodb://localhost", "Scheduler");
            StartOptions options = new StartOptions();
            options.Urls.Add("http://localhost:9095");
            options.Urls.Add("http://127.0.0.1:9095");

            // remember this! C:\WINDOWS\system32 > netsh http add urlacl url = http://127.0.0.1:9095/ user=Everyone listen=yes

            WebApp.Start<Startup>(options);

            RecurringJob.AddOrUpdate(() => Console.WriteLine("Recurring!"),Cron.Minutely());

          //  _timer = new System.Timers.Timer(1000) { AutoReset = true };
          //  _timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and all is well", DateTime.Now);
        }
        public void Start() {
            //_timer.Start(); 
        }
        public void Stop() {
            //_timer.Stop();
        }
    }
}
