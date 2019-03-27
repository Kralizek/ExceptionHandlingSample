using ExceptionHandlingSample.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExceptionHandlingSample
{
    public class Startup
    {
        // We add IHostingEnvironment to the constructor
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(ConfigureMvc).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            void ConfigureMvc(MvcOptions options)
            {
                if (HostingEnvironment.IsProduction())
                {
                    options.Filters.Add<ExceptionHandlerFilter>(); // we register our filter globally, but only if on production!
                }
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
