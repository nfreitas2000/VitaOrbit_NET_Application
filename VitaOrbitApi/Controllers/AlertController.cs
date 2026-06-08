using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaOrbitApi.Data;
using VitaOrbitApi.Models;

namespace VitaOrbitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlertController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAllAlerts()
        {
            var alerts = await _context.Alerts.Include(a => a.User).Include(a => a.HealthRecord).Include(a => a.SymptomRecord).OrderByDescending(a => a.RegisteredAt).ToListAsync();
            
            return Ok(alerts);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Alert>> GetAlertById(int id)
        {
            var alert = await _context.Alerts.Include(a => a.User).Include(a => a.HealthRecord).Include(a => a.SymptomRecord).FirstOrDefaultAsync(a => a.AlertId == id);

            if (alert == null)
                return NotFound("Alerta não encontrado.");

            return Ok(alert);
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsByUserId(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            var alerts = await _context.Alerts.Where(a => a.UserId == userId).Include(a => a.HealthRecord).Include(a => a.SymptomRecord).OrderByDescending(a => a.RegisteredAt).ToListAsync();

            return Ok(alerts);
        }


        [HttpPost]
        public async Task<ActionResult<Alert>> CreateAlert([FromBody] Alert alert)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == alert.UserId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            if (alert.HealthRecordId == null && alert.SymptomRecordId == null)
                return BadRequest("O alerta deve estar vinculado a um registro de saúde ou a um registro de sintoma.");

            if (alert.HealthRecordId != null && alert.SymptomRecordId != null)
                return BadRequest("O alerta deve estar vinculado a apenas um tipo de registro.");

            if (alert.HealthRecordId != null)
            {
                var healthRecordExists = await _context.HealthRecords.AnyAsync(h => h.HealthRecordId == alert.HealthRecordId && h.UserId == alert.UserId);

                if (!healthRecordExists)
                    return BadRequest("Registro de saúde não encontrado para este usuário.");
            }

            if (alert.SymptomRecordId != null)
            {
                var symptomRecordExists = await _context.SymptomRecords.AnyAsync(s => s.SymptomRecordId == alert.SymptomRecordId && s.UserId == alert.UserId);

                if (!symptomRecordExists)
                    return BadRequest("Registro de sintoma não encontrado para este usuário.");
            }

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlertById), new { id = alert.AlertId }, alert);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAlert(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);

            if (alert == null)
                return NotFound("Alerta não encontrado.");

            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();

            return Ok("Alerta deletado com sucesso.");
        }



    }
}
