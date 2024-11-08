namespace Cowork.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public DateTime DataReserva { get; set; }
        public TimeSpan HorarioInicio { get; set; }
        public TimeSpan HorarioFim { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int SalaId { get; set; }
        public Sala? Sala { get; set; }
        public ICollection<Funcionario>? Funcionarios { get; set; }
    }
}
