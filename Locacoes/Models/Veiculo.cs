﻿namespace Locacoes.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public int AnoFabricacao { get; set; }
        public string Combustivel { get; set; }
        public long Kilometragem { get; set; }
        public int ModeloId { get; set; }
        public Modelo Modelo { get; set; }
    }
}