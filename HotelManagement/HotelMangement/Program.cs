using Hotel.BusinessLogic.Services;
using Hotel.DataObject.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading;

namespace HotelMangement
{
    class Program
    {
        static Serilog.Core.Logger logger;
        static void Main(string[] args)
        {
            var svc = AppStartup();
            while (true)
            {
                Console.Write("Please enter full input path: ");
                var inputPath = Console.ReadLine();
                if (string.IsNullOrEmpty(inputPath))
                {
                    Console.WriteLine("Full input path cannot be null or empty.");
                }
                else
                {
                    var isSuccess = svc.Run(inputPath, out string outputPath, out string msg);
                    Log.CloseAndFlush();
                    Thread.Sleep(5);
                    if (isSuccess)
                    {
                        Console.WriteLine($"Please check result at: {outputPath}");
                    }
                    else
                    {
                        Console.WriteLine(msg);
                    }
                }
                Console.WriteLine("Do you want to continue, please enter [Y/N]?");
                var isContinue = Convert.ToString(Console.ReadLine()).ToLower().Equals("y");
                if (!isContinue)
                {
                    break;
                }
            }
        }

        static HotelService AppStartup()
        {
            var config = GetConfig();
            logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .CreateLogger();
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(config);
                builder.AddConsole();
                builder.AddSerilog(logger);
            });
            services.Configure<AppSettings>(config.GetSection(nameof(AppSettings)));
            services.AddTransient<Hotel.DataObject.Models.Hotel>();
            services.AddScoped<DefaultHotel>();
            services.AddScoped<HotelBuilder>((serviceProvider) => serviceProvider.GetRequiredService<DefaultHotel>());
            services.AddTransient<HotelCreator>();
            services.AddTransient<HotelService>();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<HotelService>();


        }

        private static IConfigurationRoot GetConfig()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            return configuration;
        }
    }
}
