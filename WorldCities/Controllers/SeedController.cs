using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Data;
using WorldCities.Data.Models;

namespace WorldCities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SeedController> _logger;

        public SeedController(
            ILogger<SeedController> logger,
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            var path = Path.Combine(_env.ContentRootPath, string.Format("Data/Source/worldcities.xlsx"));
            
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var ep = new ExcelPackage(stream);

            var ws = ep.Workbook.Worksheets[0];
            var nCountries = 0;
            var nCities = 0;

            // create a list containing all the countries
            // already existing into the Database (it
            // will be empty on first run).
            var lstCountries = _context.Countries.ToList();

            // iterates through all rows, skipping the
            // first one
            for (int nRow = 2; nRow <= ws.Dimension.End.Row; nRow++)
            {
                var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
                var countryName = row[nRow, 5].GetValue<string>();

                // Did we already created a country with that name?
                if (lstCountries.Where(c => c.Name == countryName).Count() == 0)
                {
                    // create the Country entity and fill it with xlsx data
                    var country = new Country();
                    country.Name = countryName;
                    country.ISO2 = row[nRow, 6].GetValue<string>();
                    country.ISO3 = row[nRow, 7].GetValue<string>();
                    // save it into the Database
                    await _context.Countries.AddAsync(country);
                    await _context.SaveChangesAsync();
                    // store the country to retrieve its Id later on
                    lstCountries.Add(country);
                    // increment the counter
                    nCountries++;

                    _logger.LogInformation($"country added ({nRow}/{ws.Dimension.End.Row}): {country.Name}...");
                }
            }

            var countryDict = lstCountries.ToDictionary(c => c.Name, c => c);
            var cities = new List<City>();

            // iterates through all rows, skipping the first one
            for (int nRow = 2; nRow <= ws.Dimension.End.Row; nRow++)
            {
                var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
                // create the City entity and fill it with xlsx data
                var city = new City();
                city.Name = row[nRow, 1].GetValue<string>();
                city.Name_ASCII = row[nRow, 2].GetValue<string>();
                city.Lat = row[nRow, 3].GetValue<decimal>();
                city.Lon = row[nRow, 4].GetValue<decimal>();
                // retrieve CountryId
                var countryName = row[nRow, 5].GetValue<string>();
                var country = countryDict[countryName];
                city.CountryId = country.Id;
                // save the city into the temp collection
                cities.Add(city);
                // increment the counter
                nCities++;

                _logger.LogInformation($"city loading ({nRow}/{ws.Dimension.End.Row}): {city.Name_ASCII}...");

                // TODO remove this
                if (nRow > 250) break;
            }

            // add the cities all at once
            _logger.LogInformation($"bulk adding {nCities} cities...");
            await _context.AddRangeAsync(cities);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"finished adding {nCities} cities...");

            return new JsonResult(new
            {
                Cities = nCities,
                Countries = nCountries
            });
        }
    }
}