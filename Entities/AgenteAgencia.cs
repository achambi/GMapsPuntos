using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class AgenteAgencia : Punto
    {   
        public int            AgenteAgenciaId   { get; set; }
        public String         NombreServidor    { get; set; }
        public List<Horario>  ListHorarios      { get; set; }
    }
}
