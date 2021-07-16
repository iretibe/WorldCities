using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities.Angular.Data;
using WorldCities.Angular.Data.Models;

namespace WorldCities.Angular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        // GET: api/Cities/?pageIndex=0&pageSize=10
        // GET: api/Cities/0/10
        [HttpGet]
        //[Route("{pageIndex?}/{pageSize?}")]
        public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(int pageIndex = 0, int pageSize = 10,
            string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
        {
            return await ApiResult<CityDTO>
                .CreateAsync(
                _context
                    .Cities
                    .Select(c => new CityDTO() 
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Lat = c.Lat,
                        Lon = c.Lon,
                        CountryId = c.Country.Id,
                        CountryName = c.Country.Name
                    }), 
                pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }


        //// GET: api/Cities
        //// GET: api/Cities/?pageIndex=0&pageSize=10
        //// GET: api/Cities/0/10
        //[HttpGet]
        ////[Route("{pageIndex?}/{pageSize?}")]
        //public async Task<ActionResult<ApiResult<City>>> GetCities(int pageIndex = 0, int pageSize = 10,
        //    string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
        //{
        //    return await ApiResult<City>
        //        .CreateAsync(_context.Cities, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        //}

        //// GET: api/Cities
        //// GET: api/Cities/?pageIndex=0&pageSize=10
        //// GET: api/Cities/0/10
        //[HttpGet]
        ////[Route("{pageIndex?}/{pageSize?}")]
        //public async Task<ActionResult<ApiResult<City>>> GetCities(int pageIndex = 0, int pageSize = 10, 
        //    string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
        //{
        //    //We first perform the filtering...
        //    var cities = _context.Cities;

        //    if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery))
        //    {
        //        cities = (DbSet<City>)cities.Where(c => c.Name.Contains(filterQuery));
        //    }

        //    // ... and then we call the ApiResult
        //    return await ApiResult<City>.CreateAsync(cities, pageIndex, pageSize, sortColumn, sortOrder);
        //}

        //// GET: api/Cities
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<City>>> GetCities(int pageIndex = 0, int pageSize = 10)
        //{
        //    return await _context.Cities
        //        .Skip(pageIndex * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //}

        //// GET: api/Cities
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<City>>> GetCities()
        //{
        //    return await _context.Cities.ToListAsync();
        //}

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return city;
        }

        [HttpPost] 
        [Route("IsDupeCity")] 
        public bool IsDupeCity(City city) 
        { 
            return _context.Cities.Any(e => e.Name == city.Name 
                && e.Lat == city.Lat 
                && e.Lon == city.Lon 
                && e.CountryId == city.CountryId 
                && e.Id != city.Id); 
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }
            _context.Entry(city).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
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

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCity", new { id = city.Id }, 
             city);
        }

        // DELETE: api/Cities/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<City>> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}