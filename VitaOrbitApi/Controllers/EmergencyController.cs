using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaOrbitApi.Data;
using VitaOrbitApi.Models;

namespace VitaOrbitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmergencyController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emergency>>> GetAllEmergencies()
        {
            var emergencies = await _context.Emergencies.Include(e => e.User).OrderByDescending(e => e.RequestDate).ToListAsync();

            return Ok(emergencies);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Emergency>> GetEmergencyById(int id)
        {
            var emergency = await _context.Emergencies.Include(e => e.User).FirstOrDefaultAsync(e => e.EmergencyId == id);

            if (emergency == null)
                return NotFound("Emergência não encontrada.");

            return Ok(emergency);
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Emergency>>> GetEmergenciesByUserId(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            var emergencies = await _context.Emergencies.Where(e => e.UserId == userId).OrderByDescending(e => e.RequestDate).ToListAsync();

            return Ok(emergencies);
        }


        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateEmergencyStatus(int id, [FromBody] string status)
        {
            var emergency = await _context.Emergencies.FindAsync(id);

            if (emergency == null)
                return NotFound("Emergência não encontrada.");

            if (string.IsNullOrWhiteSpace(status))
                return BadRequest("O status é obrigatório.");

            emergency.UpdateStatus(status);
            await _context.SaveChangesAsync();

            return Ok("Status da emergência atualizado com sucesso.");
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmergency(int id)
        {
            var emergency = await _context.Emergencies.FindAsync(id);

            if (emergency == null)
                return NotFound("Emergência não encontrada.");

            _context.Emergencies.Remove(emergency);
            await _context.SaveChangesAsync();

            return Ok("Emergência deletada com sucesso.");
        }

    }
}
