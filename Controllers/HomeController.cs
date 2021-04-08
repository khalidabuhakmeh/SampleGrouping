using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleGrouping.Models;
using SampleGrouping.Models.Home;

namespace SampleGrouping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext database;

        public HomeController(ILogger<HomeController> logger, AppDbContext database)
        {
            _logger = logger;
            this.database = database;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel();

            // filter elements by database query
            var results =
                await database
                    .Overrides
                    .Select(o => new
                    {
                        o.PropertyId,
                        o.Bymmddyyyy,
                        o.OverrideAmount,
                        o.Id
                    })
                    // put where clause, will also 
                    // need to determine completeness of data
                    // "did we retrieve all of a properties rows?"
                    .ToListAsync();

            // group rows in memory
            // not possible with SQL because complex collection
            var rows = results
                .GroupBy(o => o.PropertyId)
                .Select(g => new PropertyModel
                {
                    PropertyId = g.Key,
                    // 2021 JAN
                    YearMonths = g
                        .GroupBy(gg => gg.Bymmddyyyy.ToString("yyyy MMM"))
                        .Select(gg => new YearMonthModel
                        {
                            YearMonth = gg.Key,
                            Amounts = gg.Select(x => new OverrideModel
                            {
                                OverrideId = x.Id,
                                Amount = x.OverrideAmount
                            }).ToList() // FirstOrDefault for one
                        }).ToList()
                })
                // have mercy on the JSON renderer
                .Take(100)
                .ToList();
            
            /*
             * Final Results look like
             * Property
             *  |-> YearMonths (2021 Jan, 2021 Feb, etc...)
             *      |-> Amounts (Override Id along with Amounts*)
             *          (Change ToList to First to get one amount if the data has that shape)
             */

            model.Rows = rows;

            // you now can return to View instead
            // and generate your HTML using a much
            // simpler logical model
            return Json(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}