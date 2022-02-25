using Castle.Windsor;
using CIU_WPF.AppLayer;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Windows;

namespace CIU_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            Serilog.Core.Logger log = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .WriteTo.File(
                path: AppDomain.CurrentDomain.BaseDirectory, 
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose, 
                rollingInterval: RollingInterval.Day, 
                rollOnFileSizeLimit: true)
           .CreateLogger();

            
            var ioc = new WindsorContainer();
            ioc.Register(Castle.MicroKernel.Registration.Component.For<Serilog.ILogger>().Instance(log));
            //ioc.Register(Castle.MicroKernel.Registration.Component.For < SomeDependency...> ().ImplementedBy < SomeDependency Implementation...> ());
            ioc.Register(Castle.MicroKernel.Registration.Component.For<ImageViewerWindow>().ImplementedBy<ImageViewerWindow>());
            ioc.Register(Castle.MicroKernel.Registration.Component.For<ImageEditWindow>().ImplementedBy<ImageEditWindow>());
            ioc.Register(Castle.MicroKernel.Registration.Component.For<ImageBrowserWindow>().ImplementedBy<ImageBrowserWindow>());
            log.Information("Application Started");

            var ivwLog = ioc.Resolve<Serilog.ILogger>();
            AppLauncher.Instance.LaunchByArgs(e.Args, ivwLog);
        }
    }
}
