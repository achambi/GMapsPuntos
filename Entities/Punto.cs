using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Punto
    {
        public int      PuntoId          { get; set; }
        public String   Nombre           { get; set; }
        public int      TipoPuntoId      { get; set; }
        public String   TipoPuntoDesc    { get; set; }
        public int      DepartamentoId   { get; set; }
        public String   DepartamentoDesc { get; set; }
        public String   IconUrl          { get; set; }
        public Decimal  Latitud          { get; set; }
        public Decimal  Longitud         { get; set; }
        public String   Direccion        { get; set; }
        public String   htmlDescripcion  { get; set; }
    }

    public class ListPunto:Resultado
    {
        public List<Punto> listPunto { get; set; }
    }
}
