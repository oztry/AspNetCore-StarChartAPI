using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject obj)
        {
            _context.CelestialObjects.Add(obj);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", routeValues: new { id = obj.Id }, value: obj);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject obj)
        {
            var existingObj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (existingObj == null) return NotFound();
            existingObj.Name = obj.Name;
            existingObj.OrbitalPeriod = obj.OrbitalPeriod;
            existingObj.OrbitedObjectId = obj.OrbitedObjectId;
            _context.CelestialObjects.Update(existingObj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (obj == null) return NotFound();
            obj.Name = name;
            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objects = new List<CelestialObject>();
            var OrbitedObjects = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
            if (OrbitedObjects.Any()) objects.AddRange(OrbitedObjects);
            var obj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (obj != null) objects.Add(obj);
            if (!objects.Any()) return NotFound();
            _context.CelestialObjects.RemoveRange(objects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
