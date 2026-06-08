using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaOrbitApi.Data;
using VitaOrbitApi.Models;

namespace VitaOrbitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthRecordController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthRecordController(AppDbContext context)
        {
            _context = context;

        }

        private string ClassifyRisk(HealthRecord record)
        {
            if (record.OxygenSaturation < 90 || record.BodyTemperature >= 39 || record.HeartRate > 130)
            {
                return "Crítico";
            }

            if (record.OxygenSaturation < 94 || record.BodyTemperature >= 38 || record.HeartRate > 110 || record.SystolicPressure > 150)
            {
                return "Alto";
            }

            if (record.BodyTemperature >= 37.5m || record.HeartRate > 100 || record.SleepHours < 5)
            {
                return "Moderado";
            }

            return "Baixo";
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<HealthRecord>>> GetAllHealthRecords()
        {
            var records = await _context.HealthRecords.Include(h => h.User).ToListAsync();
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HealthRecord>> GetHealthRecordById(int id)
        {
            var record = await _context.HealthRecords.Include(h => h.User).FirstOrDefaultAsync(h => h.HealthRecordId == id);

            if (record == null)
                return NotFound("Registro de saúde não encontrado.");

            return Ok(record);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<HealthRecord>>> GetHealthRecordsByUserId(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            var records = await _context.HealthRecords.Where(h => h.UserId == userId).OrderByDescending(h => h.RegisteredAt).ToListAsync();

            return Ok(records);
        }


        [HttpPost]
        public async Task<ActionResult<HealthRecord>> CreateHealthRecord([FromBody] HealthRecord healthRecord)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == healthRecord.UserId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            healthRecord.UpdateRiskClassification(ClassifyRisk(healthRecord));

            _context.HealthRecords.Add(healthRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHealthRecordById),new { id = healthRecord.HealthRecordId },healthRecord);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHealthRecord(int id)
        {
            var record = await _context.HealthRecords.FindAsync(id);

            if (record == null)
                return NotFound("Registro de saúde não encontrado.");

            _context.HealthRecords.Remove(record);
            await _context.SaveChangesAsync();

            return Ok("Registro de saúde deletado com sucesso."); ;
        }
    }
}
