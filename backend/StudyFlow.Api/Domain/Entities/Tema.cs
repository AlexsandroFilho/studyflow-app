using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyFlow.Api.Domain.Entities
{
    public class Tema
    {
        public int  Id { get; set; }
        public String? Nome {get; set;}
        public String? Descricao { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public ICollection<Nota> Notas { get; set; } = new List<Nota>();
        
    }
}