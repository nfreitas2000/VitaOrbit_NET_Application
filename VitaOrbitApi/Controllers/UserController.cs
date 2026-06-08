using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VitaOrbitApi.Data;
using VitaOrbitApi.Models;

namespace VitaOrbitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController: ControllerBase 
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.Include(u => u.HealthRecords).ToListAsync();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.Include(u => u.HealthRecords).FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            return Ok(user);
        }


        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            var emailAlreadyExists = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (emailAlreadyExists)
                return BadRequest("Já existe um usuário cadastrado com este e-mail.");

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] JsonElement body)
        {
            var email = body.GetProperty("email").GetString();
            var password = body.GetProperty("password").GetString();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return BadRequest("E-mail e senha são obrigatórios.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
                return Unauthorized("E-mail ou senha inválidos.");

            return Ok(new
            {
                message = "Login realizado com sucesso.",
                userId = user.UserId,
                fullName = user.FullName,
                email = user.Email
            });
        }


        [HttpPut("{id}/email")]
        public async Task<ActionResult> UpdateEmail(int id, [FromBody] JsonElement body)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            var email = body.GetProperty("email").GetString();

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("O e-mail é obrigatório.");

            user.UpdateEmail(email);

            await _context.SaveChangesAsync();

            return Ok("E-mail alterado com sucesso.");
        }


        [HttpPut("{id}/phone")]
        public async Task<ActionResult> UpdatePhoneNumber(int id, [FromBody] JsonElement body)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            var phoneNumber = body.GetProperty("phoneNumber").GetString();

            if (string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest("O telefone é obrigatório.");

            user.UpdatePhoneNumber(phoneNumber);

            await _context.SaveChangesAsync();

            return Ok("Telefone alterado com sucesso.");
        }


        [HttpPut("{id}/location")]
        public async Task<ActionResult> UpdateCurrentLocation(int id, [FromBody] JsonElement body)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            var currentLocation = body.GetProperty("currentLocation").GetString();

            if (string.IsNullOrWhiteSpace(currentLocation))
                return BadRequest("A localização é obrigatória.");

            user.UpdateCurrentLocation(currentLocation);

            await _context.SaveChangesAsync();

            return Ok("Localização alterada com sucesso.");
        }


        [HttpPut("{id}/emergency-contact")]
        public async Task<ActionResult> UpdateEmergencyContact(int id, [FromBody] JsonElement body)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            var emergencyContact = body.GetProperty("emergencyContact").GetString();

            if (string.IsNullOrWhiteSpace(emergencyContact))
                return BadRequest("O telefone de emergência é obrigatório.");

            user.UpdateEmergencyContact(emergencyContact);

            await _context.SaveChangesAsync();

            return Ok("Telefone de emergência alterado com sucesso.");
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuário não encontrado.");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("Usuário deletado com sucesso.");
        }


        

    }
}
