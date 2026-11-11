using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SimuladoOficina.Api.Data;
using SimuladoOficina.Api.Models;

namespace SimuladoOficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDataProtector _protetor;

        public PersonalsController(
            AppDbContext context,
            IDataProtectionProvider provider)
        {
            _context = context;
            _protetor = provider.CreateProtector("IronFit.Personal.CPF");
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonals(string? busca)
        {
            var personais = _context.Personals.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                personais = personais.Where(p => p.Nome.Contains(busca));
            }

            var resultado = await personais.ToListAsync();

            var lista = resultado.Select(p => new
            {
                p.Id,
                p.Nome,
                Cpf = _protetor.Unprotect(p.Cpf)
            });

            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersonal(PersonalDto dto)
        {
            var personal = new Personal
            {
                Nome = dto.Nome,
                Cpf = _protetor.Protect(dto.Cpf)
            };

            _context.Personals.Add(personal);
            await _context.SaveChangesAsync();

            return Ok("Personal cadastrado com sucesso!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonal(int id, PersonalDto dto)
        {
            var personal = await _context.Personals.FindAsync(id);

            if (personal == null)
            {
                return NotFound("Personal não encontrado.");
            }

            personal.Nome = dto.Nome;
            personal.Cpf = _protetor.Protect(dto.Cpf);

            await _context.SaveChangesAsync();

            return Ok("Personal atualizado com sucesso!");
        }
    }

    public class PersonalDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
    }
}
