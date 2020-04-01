using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace IPInfoService
{
    public class Startup
    {
        public static OpenAPIs.openapiconfig OpenAPIConfig;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            OpenAPIConfig = (new ConfigTool.ConfigReader<OpenAPIs.openapiconfig>("openapiconfig.json", new OpenAPIs.openapiconfig() { config = new List<OpenAPIs.hostconfig>() })).Config;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers();

            //reagan - swagger add
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "IPInfoService", Version = "v1" });
                //add custom
                c.SchemaFilter<IPInfoService.Swagger.SchemaFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            //reagan - swagger add
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IPInfoService API v1"); });

        }
    }
}