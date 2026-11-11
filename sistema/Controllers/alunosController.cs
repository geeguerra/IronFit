using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimuladoOficina.Api.Data;
using SimuladoOficina.Api.Models;

namespace SimuladoOficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlunosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlunos(string? busca)
        {
            var alunos = _context.Alunos
                .Include(a => a.Personal)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                alunos = alunos.Where(a => a.Nome.Contains(busca));
            }

            var resultado = await alunos
                .Select(a => new
                {
                    a.Id,
                    a.Nome,
                    a.Cpf,
                    a.PersonalId,
                    NomePersonal = a.Personal!.Nome
                })
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAluno(AlunoDto dto)
        {
            var personal = await _context.Personals
                .FindAsync(dto.PersonalId);

            if (personal == null)
            {
                return BadRequest("Personal não encontrado.");
            }

            var aluno = new Aluno
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                PersonalId = dto.PersonalId
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            return Ok("Aluno cadastrado com sucesso!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAluno(int id, AlunoDto dto)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
            {
                return NotFound("Aluno não encontrado.");
            }

            var personal = await _context.Personals
                .FindAsync(dto.PersonalId);

            if (personal == null)
            {
                return BadRequest("Personal não encontrado.");
            }

            aluno.Nome = dto.Nome;
            aluno.Cpf = dto.Cpf;
            aluno.PersonalId = dto.PersonalId;

            await _context.SaveChangesAsync();

            return Ok("Aluno atualizado com sucesso!");
        }
    }

    public class AlunoDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public int PersonalId { get; set; }
    }
}
