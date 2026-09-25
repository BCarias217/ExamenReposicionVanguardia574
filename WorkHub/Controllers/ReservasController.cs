using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkHub.Data;
using WorkHub.Models;

namespace WorkHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly WorkHubDbContext _context;

        public ReservasController(WorkHubDbContext context)
        {
            _context = context;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas.ToListAsync();
        }

        // GET: api/Reservas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
                return NotFound();

            return reserva;
        }

        // POST: api/Reservas
        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReserva), new { id = reserva.Id }, reserva);
        }
    }
}