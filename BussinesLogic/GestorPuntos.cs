using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Auditoria;
using NLog;
using Entities;
using DataAccess;
namespace BussinesLogic
{
    public class GestorPuntos
    {
        /// <summary>
        /// Adiciona Nuevos
        /// </summary>
        /// <returns>True si fue exitoso y False si no se pudo insertar</returns>
        public Resultado AddPunto(Punto punto)
        {
            try
            {
                var conector = new ConectorPuntos();
                return (!conector.InsertPunto(punto)) ? new Resultado { success = false, message = "Existio un error al realizar la insercion" } : new Resultado { success = true, message = "Se inserto Correctamente" };
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(),ex, "Error En el metodo: InsertDireccionCliente");
                return new Resultado { success = false, message = "Existio un error tecnico al realizar la insercion del punto" };
            }
        }

        /// <summary>
        /// Trae el punto de origen del usuario de acuerdo a su registro de ubicacion de trabajo
        /// </summary>
        /// <returns>Data Table con los datos de la ubicacion</returns>
        public ListPunto GetManyPuntoByTipoId(int tipoPuntoId,int departamentoId)
        {
            ListPunto listPunto;
            try
            {
                var conector = new ConectorPuntos();
                DataTable dtPuntos  = conector.GetManyPuntoByTipoId(tipoPuntoId, departamentoId);
                listPunto               = new ListPunto { success = true, message = String.Empty };
                listPunto.listPunto = new List<Punto>();
                foreach (var item in from DataRow row in dtPuntos.Rows select new Punto
                {
                    PuntoId = Convert.ToInt32(row["PUNTO_ID_IN"]),
                    Nombre = Convert.ToString(row["NOMBRE_NV"]),
                    TipoPuntoDesc = Convert.ToString(row["TIPO_PUNTO"]),
                    IconUrl = Convert.ToString(row["URL_IMAGE"]),
                    DepartamentoDesc = Convert.ToString(row["DEPARTAMENTO"]),
                    Latitud = Convert.ToDecimal(row["LATITUD_DC"]),
                    Longitud = Convert.ToDecimal(row["LONGITUD_DC"]),
                    Direccion = Convert.ToString(row["DIRECCION_NV"]),
                    htmlDescripcion = Convert.ToString(row["HTML_DESCRIPCION"])
                })
                {
                    listPunto.listPunto.Add(item);
                }
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: getManyPuntoByTipoId");
                listPunto = new ListPunto { success = false, message = "Error al realizar la consulta" };
            }
             return listPunto;
        }        
    }
}
