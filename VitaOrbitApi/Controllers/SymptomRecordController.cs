using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaOrbitApi.Data;
using VitaOrbitApi.Models;

namespace VitaOrbitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SymptomRecordController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SymptomRecordController(AppDbContext context)
        {
            _context = context;

        }

        private string ClassifyRisk(SymptomRecord record)
        {
            var symptom = record.SymptomName.ToLower();

            if (record.Intensity >= 9 ||
                symptom.Contains("falta de ar") ||
                symptom.Contains("dor no peito") ||
                symptom.Contains("desmaio"))
            {
                return "Crítico";
            }

            if (record.Intensity >= 7 ||
                symptom.Contains("febre") ||
                symptom.Contains("tontura intensa") ||
                symptom.Contains("confusão") ||
                symptom.Contains("dor de cabeça"))
            {
                return "Alto";
            }

            if (record.Intensity >= 4)
            {
                return "Moderado";
            }

            return "Baixo";
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<SymptomRecord>>> GetAllSymptomRecords()
        {
            var records = await _context.SymptomRecords.Include(s => s.User).ToListAsync();
            return Ok(records);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SymptomRecord>> GetSymptomRecordById(int id)
        {
            var record = await _context.SymptomRecords.Include(s => s.User).FirstOrDefaultAsync(s => s.SymptomRecordId == id);

            if (record == null)
                return NotFound("Registro de sintoma não encontrado.");

            return Ok(record);
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<SymptomRecord>>> GetSymptomRecordsByUserId(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            var records = await _context.SymptomRecords.Where(s => s.UserId == userId).OrderByDescending(s => s.RegisteredAt).ToListAsync();

            return Ok(records);
        }


        [HttpPost]
        public async Task<ActionResult<SymptomRecord>> CreateSymptomRecord([FromBody] SymptomRecord symptomRecord)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == symptomRecord.UserId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            symptomRecord.UpdateRiskClassification(ClassifyRisk(symptomRecord));

            _context.SymptomRecords.Add(symptomRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSymptomRecordById), new { id = symptomRecord.SymptomRecordId }, symptomRecord);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSymptomRecord(int id)
        {
            var record = await _context.SymptomRecords.FindAsync(id);

            if (record == null)
                return NotFound("Registro de sintoma não encontrado.");

            _context.SymptomRecords.Remove(record);
            await _context.SaveChangesAsync();

            return Ok("Registro de sintoma deletado com sucesso."); ;
        }
    }



}

