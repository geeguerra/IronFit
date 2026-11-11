namespace SimuladoOficina.Api.Models
{
    public class Aluno
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }

        public int PersonalId { get; set; }
        public Personal? Personal { get; set; }
    }
}