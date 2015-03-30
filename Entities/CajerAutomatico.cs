using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class CajerAutomatico:Punto
    {
        public int      CajeroId         { get; set; }
        public Boolean  FlagMinusvalido  { get; set; }
        public int      MonedaId         { get; set; }        
    }

    public class ListCajerAutomatico : Resultado
    {
        public List<CajerAutomatico> listCajerAutomatico { get; set; }
    }
}
