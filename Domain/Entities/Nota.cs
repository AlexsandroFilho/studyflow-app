using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyFlow.Api.Domain.Entities
{
    public class Nota
    {
        public int Id { get; set; }
        public String? Titulo { get; set; }
        public String? Conteudo { get; set; }
        public String? ResumoIA { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public int TemaId { get; set; }
        public Tema? Tema { get; set; }

        public ICollection<ConexaoNota> ConexoesOrigem {get; set;} = new List<ConexaoNota>();
        public ICollection<ConexaoNota> ConexoesDestino {get; set;} = new List<ConexaoNota>();
    }
}