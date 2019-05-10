using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using CityInfo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CityInfo
{
    public class Startup
    {
        public static IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // IN asp.net core 2 CreateDefaultBuilder already includes:
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appSettings.json", optional:false, reloadOnChange:true);

            //Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // So that the json will serialize the property names as they are aka NOT CAMEL CASE
            //services.AddMvc().AddJsonOptions(o=> {
            //    if(o.SerializerSettings.ContractResolver !=null)
            //    {
            //        var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
            //        castedResolver.NamingStrategy = null;
            //    }
            //});

            // Default output to camel case json
            // Add XML format, return in XML if user request XML
            services.AddMvc().AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter()));
#if DEBUG
            services.AddTransient<IMailService, LocalMailServices>();
            // Because we using debug.writeline, release wont work?
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            //  services.AddDbContext<Entities.CityInfoContext>();
            // var connectionString = @"Server=(localdb)\mssqllocaldb;Database=CityInfoDB;Trusted_Connection=True;";
            var connectionString = Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<Entities.CityInfoContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<ICityInfoRepository, CityInfoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, Entities.CityInfoContext cityInfoContext
            )
        {

            // Next 2 lines does same thing
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            loggerFactory.AddNLog();

            // Middleware for development env only
            if (env.IsDevelopment())
            {
                // Middleware that handles exception
                app.UseDeveloperExceptionPage();
            }

            cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(cfg => {
                // Map From name to same name
                // ignore variables that doesnt exists
                cfg.CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
                cfg.CreateMap<Entities.City, Models.CityDto>();
                cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
                cfg.CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>();
                cfg.CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
                cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>();
            });

            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
