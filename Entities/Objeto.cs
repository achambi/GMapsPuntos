using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Objeto
    {
        public DateTime fechaModificacion { get; set; }
        public DateTime fechaCreacion { get; set; }
        public Boolean estado { get; set; }
        public String ultimoUsuario { get; set; }
    }
}
