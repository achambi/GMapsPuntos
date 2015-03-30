using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{

    public class ListParametro:Resultado
    {
        public List<Parametro> listParametro { get; set; }
    }

    public class Parametro
    {
        public long     parametroId         { get; set; }
        public long     parametroCodigo     { get; set; }
        public string   tipoParametro       { get; set; }
        public string   valorParametroCorto { get; set; }
        public string   valorParametro      { get; set; } 
    }
}
