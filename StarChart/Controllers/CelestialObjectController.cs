using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (obj == null) return NotFound();
            obj.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
            return Ok(obj);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var objects = _context.CelestialObjects.Where(c => c.Name == name).ToList();
            if (!objects.Any()) return NotFound();
            foreach (var obj in objects)
            {
                obj.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == obj.Id).ToList();
            }
            return Ok(objects);
        }


        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            var objects = _context.CelestialObjects.ToList();
            foreach(var obj in objects)
            {
                obj.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == obj.Id).ToList();
            }
            return Ok(objects);
        }
    }
}
