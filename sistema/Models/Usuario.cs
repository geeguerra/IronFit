namespace SimuladoOficina.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
        {
        public string Username { get; set; }
        public string Password { get; set; }
    }


}
