using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyFlow.Api.Domain.Entities
{
    public class ConexaoNota
    {
        public int Id { get; set; }
        public String? Rotulo { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;


        public int NotaOrigemId { get; set; }
        public Nota? NotaOrigem { get; set; }


        public int NotaDestinoId { get; set; }
        public Nota? NotaDestino {get; set;}
    }
}