using FineCollectionService.DomainServices;
using FineCollectionService.Proxies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapr.Client;

namespace FineCollectionService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFineCalculator, HardCodedFineCalculator>();

            // add service proxies
            services.AddSingleton<VehicleRegistrationService>(_ => 
                new VehicleRegistrationService(
                    // Create HttpClient instance that implements service invocation building block. You still call httpClient, but underneath the hood it uses the Dapr Service Invocation building block.
                    // Specify the app-id of the service with which to communicate
                    // Specify the address of the Dapr sidecar
                    DaprClient.CreateInvokeHttpClient("vehicleregistrationservice", "http://localhost:3601")));
            // Legacy non-Dapr code
            //services.AddHttpClient();
            //services.AddSingleton<VehicleRegistrationService>();

            // AddDapr() adds Dapr integration to ASP.NET MVC
            services.AddControllers().AddDapr();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Adds middleware into ASP.NET COre middleware pipeline to automatically unwrap CloudEvent formatted messages
            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                // MapSubscriberHandler extension implements /dapr/subscribe endpoints 
                // it returns subscriptions for controller methods decorated with the Topic attribute
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });
        }
    }
}
