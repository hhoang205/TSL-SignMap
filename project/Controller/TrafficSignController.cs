using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Controller
{
    [Route("api/trafficsings")]
    [ApiController]
    public class TrafficSignController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public TrafficSignController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var trafficsigns = _context.TrafficSigns.ToList();
            return Ok(trafficsigns);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var trafficsign = _context.TrafficSigns.Find(id);
            if (trafficsign == null)
                return NotFound();
            return Ok(trafficsign);
        }

        [HttpPost]
        public IActionResult CreateTrafficSign(TrafficSign trafficSign)
        {
            _context.TrafficSigns.Add(trafficSign);
            _context.SaveChanges();
            return Ok(trafficSign);
        }

        [HttpDelete]
        public IActionResult DeleteTrafficSign(int id)
        {
            var trafficsign = _context.TrafficSigns.Find(id);
            if (trafficsign == null)
                return NotFound();
            _context.TrafficSigns.Remove(trafficsign);
            _context.SaveChanges();
            return Ok(trafficsign);
        }
    }
}
