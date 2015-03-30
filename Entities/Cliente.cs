using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Cliente:Objeto
    {
        public int clienteId { get; set; }
        public String Nombre { get; set; }
        public String apPaterno { get; set; }
        public String apMaterno { get; set; }
        public String idc { get; set; }
        public String extensionIdc { get; set; }
    }
}
