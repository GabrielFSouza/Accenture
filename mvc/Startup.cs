using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mvc.Context;
using Repository;
using  Repository.BuscaStrategies;
using  Repository.BuscaStrategies.FornecedorFilters;
using Microsoft.EntityFrameworkCore;
using mvc.Repositories;
using mvc.Services;

namespace mvc
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
            services.AddControllersWithViews();
            services.AddDbContext<FornecedoresContext>();

            services.AddScoped<IFornecedoresRepository, FornecedoresRepository>();
            services.AddScoped<IFornecedoresBuscaStrategy, BuscarNome>();
            services.AddScoped<IFornecedoresBuscaStrategy, BuscarData>();
            services.AddScoped<IFornecedoresBuscaStrategy, BuscarCPFCNPJ>();
            services.AddScoped<IEmpresaService, EmpresaService>();
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();

            services.AddAuthentication().AddGoogle(options =>
        {
            IConfigurationSection googleAuthNSection =
                Configuration.GetSection("ExternalLogin:Google");

            options.ClientId = googleAuthNSection["ClientId"];
            options.ClientSecret = googleAuthNSection["ClientSecret"];
        });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
        IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            

            app.UseRouting();
            app.UseAuthentication();


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
            });


            serviceProvider.GetService<FornecedoresContext>().Database.Migrate();
            serviceProvider.GetService<IdentityDbContext>().Database.Migrate();
        }
    }
}
