namespace SimuladoOficina.Api.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }

        public int AlunoId { get; set; }
        public Aluno? Aluno { get; set; }

        public int PersonalId { get; set; }
        public Personal? Personal { get; set; }
    }
}