using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleGrouping.Models;

namespace SampleGrouping
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

            services.AddScoped(s =>
                new AppDbContext(Configuration.GetConnectionString("Sql"))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext database)
        {
            InitializeDatabase(database);

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
        }

        private static void InitializeDatabase(AppDbContext database)
        {
            // initialize database

            database.Database.Initialize(true);
            if (!database.Overrides.Any())
            {
                var rules = new Faker<Override>();
                var overrides =
                    rules.Rules((faker, o) =>
                    {
                        o.Account = faker.Random.AlphaNumeric(10);
                        o.Bymmddyyyy = faker.Date.Between(new DateTime(2021, 1, 1), new DateTime(2021, 12, 31));
                        o.PropertyId = faker.Random.AlphaNumeric(10);
                        o.OverrideAmount = (float?) faker.Random.Decimal(0, 1000m);
                        o.LastModifiedAt = DateTime.Now;
                        o.LastModifiedBy = faker.Internet.UserName();
                    }).Generate(10_000);

                database.Overrides.AddRange(overrides);
                // not async, because I'm a rebel
                database.SaveChanges();
            }
        }
    }
}