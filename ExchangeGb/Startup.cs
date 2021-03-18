using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeGb.Data;
using ExchangeGb.Models.Entities;
using ExchangeGb.Models.Repositories;
using ExchangeGb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Polly;

namespace ExchangeGb
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
            services.AddDbContext<ExchangeDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("ApplicationDbContext")));
            services.AddTransient<IDealRepository, DealRepository>();
            services.AddTransient<ISellOrderRepository, SellOrderRepository>();
            services.AddTransient<IBuyOrderRepository, BuyOrderRepository>();

            services.AddTransient<IAddSellOrderUseCase, DealService>();
            services.AddTransient<IAddBuyOrderUseCase, DealService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // Handle database in starting
            Policy.Handle<NpgsqlException>()
                .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(
                        attempt > 5 ? 60 : Math.Pow(2, attempt)
                    ),
                    (exception, timespan) =>
                        Console.WriteLine(
                            $"Cannot connect to the Database. Try again in {timespan.TotalSeconds} seconds."))
                .Execute(() => { MigrateDatabase(app); });
        }

        private void MigrateDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            ExchangeDbContext dbContext = scope.ServiceProvider.GetRequiredService<ExchangeDbContext>();
            dbContext.Database.Migrate();
        }
    }
}