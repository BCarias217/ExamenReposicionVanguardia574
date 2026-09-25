namespace WorkHub.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public int SalaId { get; set; }
        public Sala? Sala { get; set; }

        public string NombreResponsable { get; set; } = string.Empty;
        public string CorreoResponsable { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public int DuracionHoras { get; set; }
    }
}