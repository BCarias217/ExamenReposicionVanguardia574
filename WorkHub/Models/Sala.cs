namespace WorkHub.Models
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoSala { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public decimal PrecioPorHora { get; set; }
        public int Piso { get; set; }

        public List<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}