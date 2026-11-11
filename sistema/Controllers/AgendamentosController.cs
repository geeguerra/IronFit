using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimuladoOficina.Api.Data;
using SimuladoOficina.Api.Models;

namespace SimuladoOficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AgendamentosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAgendamento(CriarAgendamentoIronFitDto dto)
        {
            var aluno = await _context.Alunos.FindAsync(dto.AlunoId);

            if (aluno == null)
            {
                return NotFound("Aluno não encontrado.");
            }

            var agendamento = new Agendamento
            {
                DataHora = dto.DataHora,
                AlunoId = aluno.Id,
                PersonalId = aluno.PersonalId
            };

            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            return Ok(agendamento);
        }

        [HttpGet]
        public async Task<IActionResult> GetAgendamentos()
        {
            var agendamentos = await _context.Agendamentos
                .Include(a => a.Aluno)
                .Include(a => a.Personal)
                .OrderBy(a => a.DataHora)
                .Select(a => new
                {
                    a.Id,
                    a.DataHora,
                    NomeAluno = a.Aluno!.Nome,
                    NomePersonal = a.Personal!.Nome
                })
                .ToListAsync();

            return Ok(agendamentos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgendamento(int id)
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.Aluno)
                .Include(a => a.Personal)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.DataHora,
                    NomeAluno = a.Aluno!.Nome,
                    NomePersonal = a.Personal!.Nome
                })
                .FirstOrDefaultAsync();

            if (agendamento == null)
            {
                return NotFound();
            }

            return Ok(agendamento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAgendamento(
            int id, AtualizarAgendamentoDto dto)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);

            if (agendamento == null)
            {
                return NotFound();
            }

            agendamento.DataHora = dto.DataHora;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgendamento(int id)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);

            if (agendamento == null)
            {
                return NotFound();
            }

            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CriarAgendamentoIronFitDto
    {
        public DateTime DataHora { get; set; }
        public int AlunoId { get; set; }
    }

    public class AtualizarAgendamentoDto
    {
        public DateTime DataHora { get; set; }
    }
}
