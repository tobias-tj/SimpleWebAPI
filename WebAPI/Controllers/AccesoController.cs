using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Custom;
using WebAPI.Models;
using WebAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly DbpruebaContext _dbpruebaContext;
        private readonly Utilidades _utilidades;
        public AccesoController(DbpruebaContext dbpruebaContext, Utilidades utilidades)
        {
            _dbpruebaContext = dbpruebaContext;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO objeto)
        {
            var modeloUsuario = new Usuario
            {
                Nombre = objeto.Nombre,
                Correo = objeto.Correo,
                Clave = _utilidades.encriptarSHA256(objeto.Clave),
            };

            await _dbpruebaContext.AddAsync(modeloUsuario);
            await _dbpruebaContext.SaveChangesAsync();

            if(modeloUsuario.IdUsuario == 0)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSucces = false });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSucces = true });
            }

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO objeto)
        {
            var usuarioEncontrado = await _dbpruebaContext.Usuarios.Where(u =>
                u.Correo == objeto.Correo &&
                u.Clave == _utilidades.encriptarSHA256(objeto.Clave)
                ).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { isSucces = false, token = "" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { isSucces = true, token = _utilidades.generarJWT(usuarioEncontrado) });
            }

        }

    }
}
