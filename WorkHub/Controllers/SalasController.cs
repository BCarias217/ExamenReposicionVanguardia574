using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkHub.Common;
using WorkHub.Data;
using WorkHub.Models;

namespace WorkHub.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalasController : ControllerBase
{
    private const int NombreMinLength = 3;
    private const int NombreMaxLength = 60;
    private static readonly string[] TiposValidos = { "Individual", "Grupal", "SalaDeJuntas" };

    private readonly WorkHubDbContext _db;

    public SalasController(WorkHubDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var salas = await _db.Salas.ToListAsync();
        return Ok(salas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sala = await _db.Salas.FindAsync(id);
        if (sala is null) return NotFound();
        return Ok(sala);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Sala sala)
    {
        AplicarTransformacion(sala);
        var errorValidacion = ValidarSala(sala);
        if (errorValidacion is not null) return BadRequest(errorValidacion);
        _db.Salas.Add(sala);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = sala.Id }, sala);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Sala salaActualizada)
    {
        var sala = await _db.Salas.FindAsync(id);
        if (sala is null) return NotFound();
        AplicarTransformacion(salaActualizada);
        var errorValidacion = ValidarSala(salaActualizada);
        if (errorValidacion is not null) return BadRequest(errorValidacion);
        sala.Nombre = salaActualizada.Nombre;
        sala.TipoSala = salaActualizada.TipoSala;
        sala.Capacidad = salaActualizada.Capacidad;
        sala.PrecioPorHora = salaActualizada.PrecioPorHora;
        sala.Piso = salaActualizada.Piso;
        await _db.SaveChangesAsync();
        return Ok(sala);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sala = await _db.Salas.FindAsync(id);
        if (sala is null) return NotFound();

        var tieneReservas = await _db.Reservas.AnyAsync(r => r.SalaId == id);
        if (tieneReservas)
            return Conflict("No se puede eliminar la Sala porque tiene reservas registradas.");

        _db.Salas.Remove(sala);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static void AplicarTransformacion(Sala sala)
    {
        sala.Nombre = TextNormalizer.Normalizar(sala.Nombre);
    }

    private static string? ValidarSala(Sala sala)
    {
        if (string.IsNullOrWhiteSpace(sala.Nombre) || sala.Nombre.Length < NombreMinLength || sala.Nombre.Length > NombreMaxLength)
            return $"El Nombre es obligatorio y debe tener entre {NombreMinLength} y {NombreMaxLength} caracteres.";
        if (!TiposValidos.Contains(sala.TipoSala))
            return "El TipoSala debe ser 'Individual', 'Grupal' o 'SalaDeJuntas'.";
        if (sala.Capacidad <= 0)
            return "La Capacidad debe ser un entero mayor a 0.";
        if (sala.PrecioPorHora <= 0)
            return "El PrecioPorHora debe ser mayor a 0.";
        if (sala.Piso < 0)
            return "El Piso debe ser un entero mayor o igual a 0.";
        return null;
    }
}