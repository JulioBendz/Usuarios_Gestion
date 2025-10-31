using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Usuarios_Gestion.Data;
using Usuarios_Gestion.Models;

namespace Usuarios_Gestion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            // incluir Area para que el frontend pueda mostrar el nombre del área
            return await _context.Usuario
                                 .Include(u => u.Area)
                                 .ToListAsync();
        }

        // GET: api/usuarios/5
        [HttpGet("{codigo}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int codigo)
        {
            var usuario = await _context.Usuario
                                        .Include(u => u.Area)
                                        .FirstOrDefaultAsync(u => u.Codigo == codigo);

            if (usuario == null)
                return NotFound();

            return usuario;
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<Usuarios>> PostUsuario(Usuarios usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // validar que el area exista (evita FK error)
            var areaExists = await _context.Area.AnyAsync(a => a.Codigo == usuario.CodigoArea);
            if (!areaExists)
                return BadRequest($"Area con Codigo {usuario.CodigoArea} no encontrada.");

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            // devolver la entidad creada con la navegación Area incluida
            var created = await _context.Usuario
                                        .Include(u => u.Area)
                                        .FirstOrDefaultAsync(u => u.Codigo == usuario.Codigo);

            return CreatedAtAction(nameof(GetUsuario), new { codigo = usuario.Codigo }, created);
        }

        // PUT: api/usuarios/5
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutUsuario(int codigo, Usuarios usuario)
        {
            if (codigo != usuario.Codigo)
                return BadRequest();

            var existing = await _context.Usuario.FindAsync(codigo);
            if (existing == null)
                return NotFound();

            // validar que el area destino exista
            if (!await _context.Area.AnyAsync(a => a.Codigo == usuario.CodigoArea))
                return BadRequest($"Area con Codigo {usuario.CodigoArea} no encontrada.");

            // actualizar solo los campos que importan
            existing.Nombre = usuario.Nombre;
            existing.Contrasena = usuario.Contrasena;
            existing.Fecha = usuario.Fecha;
            existing.CodigoArea = usuario.CodigoArea;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuario.Any(e => e.Codigo == codigo))
                    return NotFound();
                else
                    throw;
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.Message);
            }

            return NoContent();
        }

        // DELETE: api/usuarios/5
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteUsuario(int codigo)
        {
            var usuario = await _context.Usuario.FindAsync(codigo);
            if (usuario == null)
                return NotFound();

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
