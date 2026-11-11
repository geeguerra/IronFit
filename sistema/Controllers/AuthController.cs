using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SimuladoOficina.Api.Data;
using SimuladoOficina.Api.Models;

namespace SimuladoOficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Usuario> _hasher = new();

        private readonly string chave = "IronFit_Chave_Secreta_2026_Projeto_Academia";

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            var existe = await _context.Usuarios
                .AnyAsync(u => u.Username == usuario.Username);

            if (existe)
            {
                return BadRequest("Usuário já cadastrado.");
            }

            usuario.Password = _hasher.HashPassword(usuario, usuario.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuário cadastrado com sucesso!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == login.Username);

            if (usuario == null)
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            var senhaCorreta = _hasher.VerifyHashedPassword(
                usuario, usuario.Password, login.Password);

            if (senhaCorreta == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome)
            };

            var chaveSeguranca = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(chave));

            var credenciais = new SigningCredentials(
                chaveSeguranca, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credenciais
            );

            return Ok(new
            {
                Nome = usuario.Nome,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
