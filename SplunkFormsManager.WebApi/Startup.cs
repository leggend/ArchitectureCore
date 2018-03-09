using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using SplunkFormsManager.Application.Contracts.Generic;
using SplunkFormsManager.Domain.DTOs;
using SplunkFormsManager.Domain.Entities;
using SplunkFormsManager.Application.Services.Generic;
using SplunkFormsManager.Domain.Libraries.Generic;
using SplunkFormsManager.Infrastrucutre.Repositories.Generic;
using SplunkFormsManager.Domain.RepositoryContracts.Generic;
using SplunkFormsManager.Domain.Contracts.Generic;
using SplunkFormsManager.Infrastrucutre.Context;
using Microsoft.EntityFrameworkCore;

namespace SplunkFormsManager.WebApi
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
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DataContext"))
            );

            services.AddCors();
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Splunk Forms Manager API", Version = "v1" });
            });

            services.AddAutoMapper();

            services.AddScoped<IGenericService<DummyDTO, DummyEntity>, GenericService<DummyDTO, DummyEntity>>();
            services.AddScoped<IGenericLibrary<DummyDTO, DummyEntity>, GenericLibrary<DummyDTO, DummyEntity>>();
            services.AddScoped<IGenericRepository<DummyEntity>, GenericRepository<DummyEntity>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(
                options => options
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Splunk Forms Manager API v1");
            });


            app.UseMvc();
        }
    }
}
