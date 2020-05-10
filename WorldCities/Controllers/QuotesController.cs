using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Data;
using WorldCities.Data.Models;

namespace WorldCities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Quotes/symbol
        [HttpGet("{symbol}")]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes(string symbol)
        {
            return await _context.Quotes
                .Where(q => q.Symbol == symbol)
                .OrderBy(q => q.Date)
                .ToListAsync();
        }

        // GET: api/Quotes/symbol/date
        [HttpGet("{symbol}/{date}")]
        public async Task<ActionResult<Quote>> GetQuote(string symbol, string date)
        {
            var quote = await _context.Quotes.FindAsync(symbol, date);

            if (quote == null)
            {
                return NotFound();
            }

            return quote;
        }

        // PUT: api/Quotes/symbol/date
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{symbol}/{date}")]
        public async Task<IActionResult> PutQuote(string symbol, string date, Quote quote)
        {
            if (symbol != quote.Symbol || date != quote.Date)
            {
                return BadRequest();
            }

            _context.Entry(quote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await QuoteExistsAsync(symbol, date))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Quotes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Quote>> PostQuote(Quote quote)
        {
            _context.Quotes.Add(quote);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await QuoteExistsAsync(quote.Symbol, quote.Date))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetQuotes", new { id = quote.Symbol }, quote);
        }

        // DELETE: api/Quotes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Quote>> DeleteQuote(string id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return NotFound();
            }

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return quote;
        }

        private async Task<bool> QuoteExistsAsync(string symbol, string date)
        {
            return await _context.Quotes.AnyAsync(e => e.Symbol == symbol && e.Date == date);
        }
    }
}
