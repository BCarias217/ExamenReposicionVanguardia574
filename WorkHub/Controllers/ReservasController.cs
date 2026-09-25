using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkHub.Common;
using WorkHub.Data;
using WorkHub.Models;

namespace WorkHub.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private const int NombreResponsableMinLength = 3;
    private const int NombreResponsableMaxLength = 100;
    private const int DuracionMaxima = 8;

    private readonly WorkHubDbContext _db;

    public ReservasController(WorkHubDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reservas = await _db.Reservas.ToListAsync();
        return Ok(reservas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reserva = await _db.Reservas.FindAsync(id);
        if (reserva is null) return NotFound();
        return Ok(reserva);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Reserva reserva)
    {
        AplicarTransformacion(reserva);

        var errorValidacion = ValidarReserva(reserva);
        if (errorValidacion is not null)
            return BadRequest(errorValidacion);

        var sala = await _db.Salas.FindAsync(reserva.SalaId);
        if (sala is null)
            return BadRequest("La Sala indicada no existe.");

        var reservasDeLaSala = await _db.Reservas
            .Where(r => r.SalaId == reserva.SalaId)
            .ToListAsync();

        var hayTraslape = reservasDeLaSala.Any(r =>
            TraslapeHelper.HayTraslape(reserva.FechaHoraInicio, reserva.DuracionHoras, r.FechaHoraInicio, r.DuracionHoras));

        if (hayTraslape)
            return Conflict("El horario solicitado se traslapa con otra reserva existente para esta sala.");

        if (reserva.FechaHoraInicio < DateTime.Now)
            return Conflict("No se puede reservar una Sala en una fecha y hora que ya paso.");

        _db.Reservas.Add(reserva);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = reserva.Id }, reserva);
    }

    // --- Transformación de datos ---

    private static void AplicarTransformacion(Reserva reserva)
    {
        reserva.NombreResponsable = TextNormalizer.Normalizar(reserva.NombreResponsable);
    }

    // --- Validaciones de formato ---

    private static string? ValidarReserva(Reserva reserva)
    {
        if (string.IsNullOrWhiteSpace(reserva.NombreResponsable) ||
            reserva.NombreResponsable.Length < NombreResponsableMinLength ||
            reserva.NombreResponsable.Length > NombreResponsableMaxLength)
            return $"El NombreResponsable es obligatorio y debe tener entre {NombreResponsableMinLength} y {NombreResponsableMaxLength} caracteres.";

        if (!System.Text.RegularExpressions.Regex.IsMatch(reserva.NombreResponsable, @"^[\p{L}\s'\-]+$"))
            return "El NombreResponsable solo puede contener letras, espacios, guiones o apostrofes.";

        if (string.IsNullOrWhiteSpace(reserva.CorreoResponsable) ||
            !System.Text.RegularExpressions.Regex.IsMatch(reserva.CorreoResponsable, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "El CorreoResponsable es obligatorio y debe tener un formato de correo valido.";

        if (reserva.DuracionHoras <= 0 || reserva.DuracionHoras > DuracionMaxima)
            return $"La DuracionHoras debe ser un entero mayor a 0 y menor o igual a {DuracionMaxima}.";

        return null;
    }
}