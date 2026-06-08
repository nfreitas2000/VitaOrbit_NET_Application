using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaOrbitApi.Data;
using VitaOrbitApi.Models;

namespace VitaOrbitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentalConditionController : ControllerBase 
    {
        private readonly AppDbContext _context;

        public EnvironmentalConditionController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnvironmentalCondition>>> GetAllEnvironmentalConditions()
        {
            var conditions = await _context.EnvironmentalConditions.Include(e => e.User).ToListAsync();
            
            return Ok(conditions);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EnvironmentalCondition>> GetEnvironmentalConditionById(int id)
        {
            var condition = await _context.EnvironmentalConditions.Include(e => e.User).FirstOrDefaultAsync(e => e.EnvironmentalConditionId == id);

            if (condition == null)
                return NotFound("Condição ambiental não encontrada.");

            return Ok(condition);
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<EnvironmentalCondition>> GetEnvironmentalConditionByUserId(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            var condition = await _context.EnvironmentalConditions.FirstOrDefaultAsync(e => e.UserId == userId);

            if (condition == null)
                return NotFound("Condição ambiental não encontrada para este usuário.");

            return Ok(condition);
        }


        [HttpPost]
        public async Task<ActionResult<EnvironmentalCondition>> CreateEnvironmentalCondition([FromBody] EnvironmentalCondition environmentalCondition)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == environmentalCondition.UserId);

            if (!userExists)
                return NotFound("Usuário não encontrado.");

            var userAlreadyHasCondition = await _context.EnvironmentalConditions.AnyAsync(e => e.UserId == environmentalCondition.UserId);

            if (userAlreadyHasCondition)
                return BadRequest("Este usuário já possui uma condição ambiental cadastrada.");

            _context.EnvironmentalConditions.Add(environmentalCondition);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnvironmentalConditionById),
                new { id = environmentalCondition.EnvironmentalConditionId },
                environmentalCondition
            );
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEnvironmentalCondition(int id,[FromBody] EnvironmentalCondition updatedCondition)
        {
            var condition = await _context.EnvironmentalConditions.FindAsync(id);

            if (condition == null)
                return NotFound("Condição ambiental não encontrada.");

            condition.UpdateEnvironmentalCondition(updatedCondition.ExternalTemperature, updatedCondition.Humidity, updatedCondition.Altitude, updatedCondition.AtmosphericPressure, updatedCondition.AirQuality, updatedCondition.RadiationLevel, updatedCondition.EnvironmentType);

            await _context.SaveChangesAsync();

            return Ok("Condição ambiental atualizada com sucesso.");
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEnvironmentalCondition(int id)
        {
            var condition = await _context.EnvironmentalConditions.FindAsync(id);

            if (condition == null)
                return NotFound("Condição ambiental não encontrada.");

            _context.EnvironmentalConditions.Remove(condition);
            await _context.SaveChangesAsync();

            return Ok("Condição ambiental deletada com sucesso.");
        }

    }
}
