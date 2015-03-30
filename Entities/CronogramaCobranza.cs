using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class CronogramaCobranza
    {
        public Int64    HojaRutaId      { get; set; }
        public string   EstadoUrlimage  { get; set; }
        public string   Nombre          { get; set; }
        public string   NumeroOperacion { get; set; }
        public string   DireccionCliente{ get; set; }
        public string   Moneda          { get; set; }
        public decimal  DeudaActual     { get; set; }
        public decimal  DeudaVencida    { get; set; }
        public string   DiasMora        { get; set; }
        public decimal  DeudaTotal      { get; set; }
        public string   Observaciones   { get; set; }
        public decimal  Latitud         { get; set; }
        public decimal  Longitud        { get; set; }
        
    }
}
