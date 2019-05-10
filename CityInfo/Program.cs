using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CityInfo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            //var host = new WebHostBuilder()
            //    .UseKestrel() // Cross Platform Web Server
            //    .UseContentRoot(Directory.GetCurrentDirectory()) // Base path for content(views,etc)
            //    .UseIISIntegration() // Use Iss express as default web host on Windows, Use Iss as reverse proxy server
            //    .UseStartup<Startup>()
            //    .Build();
            //host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
