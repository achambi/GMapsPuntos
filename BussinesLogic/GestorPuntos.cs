using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <param name="puntoBM"></param>
        /// <returns>True si fue exitoso y False si no se pudo insertar</returns>
        public Resultado AddPunto(Punto punto)
        {
            try
            {
                ConectorPuntos conector = new ConectorPuntos();
                return (!conector.insertPunto(punto)) ? new Resultado() { success = false, message = "Existio un error al realizar la insercion" } : new Resultado() { success = true, message = "Se inserto Correctamente" };
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(),ex, "Error En el metodo: InsertDireccionCliente");
                return new Resultado() { success = false, message = "Existio un error tecnico al realizar la insercion del punto" };
            }
        }

        /// <summary>
        /// Trae el punto de origen del usuario de acuerdo a su registro de ubicacion de trabajo
        /// </summary>
        /// <returns>Data Table con los datos de la ubicacion</returns>
        public ListPunto getManyPuntoByTipoId(int tipoPuntoId,int departamentoId)
        {
            ListPunto listPunto = null;
            DataTable dtPuntos  = null;
            try
            {
                ConectorPuntos conector = new ConectorPuntos();
                dtPuntos                = conector.getManyPuntoByTipoId(tipoPuntoId, departamentoId);
                listPunto               = new ListPunto() { success = true, message = String.Empty };
                listPunto.listPunto = new List<Punto>();
                foreach (DataRow row in dtPuntos.Rows)
                {
                    Punto item   = new Punto();
                    item.PuntoId          = Convert.ToInt32(row["PUNTO_ID_IN"]);
                    item.Nombre           = Convert.ToString(row["NOMBRE_NV"]);
                    item.TipoPuntoDesc    = Convert.ToString(row["TIPO_PUNTO"]);
                    item.IconUrl          = Convert.ToString(row["URL_IMAGE"]);
                    item.DepartamentoDesc = Convert.ToString(row["DEPARTAMENTO"]);
                    item.Latitud          = Convert.ToDecimal(row["LATITUD_DC"]);
                    item.Longitud         = Convert.ToDecimal(row["LONGITUD_DC"]);
                    item.Direccion        = Convert.ToString (row["DIRECCION_NV"]);
                    item.htmlDescripcion  = Convert.ToString(row["HTML_DESCRIPCION"]);
                    listPunto.listPunto.Add(item);
                }
            }
            catch (Exception ex)
            {
                TextLogger.LogError(LogManager.GetCurrentClassLogger(), ex, "Error En el metodo: getManyPuntoByTipoId");
                listPunto = new ListPunto() { success = false, message = "Error al realizar la consulta" };
            }
             return listPunto;
        }        
    }
}
